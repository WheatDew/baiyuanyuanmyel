﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OriginCharacter : MonoBehaviour
{
    
    #region 通用继承

    private void Start()
    {
        GeneralInitialize();
        CharacterWorkInitialize();
    }

    private void Update()
    {
        CharacterPropertyJob();
        CharacterWorkJob();
    }

    private void OnTriggerEnter(Collider other)
    {
        EnterArea.Add(other.name);
    }

    private void OnTriggerExit(Collider other)
    {
        EnterArea.Remove(other.name);
    }

    #endregion

    #region 通用功能

    //通用功能
    private OriginUserInterfaceController userInterfaceController;
    private CharacterResource characterResource;
    private OriginRaySystem originRaySystem;
    private OriginEventLib eventLib;
    private HashSet<string> EnterArea = new HashSet<string>();

    public void GeneralInitialize()
    {
        userInterfaceController = FindObjectOfType<OriginUserInterfaceController>();
        characterResource = FindObjectOfType<CharacterResource>();
        originRaySystem = FindObjectOfType<OriginRaySystem>();
        eventLib = FindObjectOfType<OriginEventLib>();
    }

    #endregion

    #region 角色属性模块

    //角色属性
    [Range(0f, 100f)] public float hungerValue, hungerRate;
    private HashSet<string> reportflag = new HashSet<string>();

    public void HungerValueRateJob()
    {
        hungerValue -= hungerRate*Time.deltaTime;
    }

    //饥饿播报循环
    public void HungerReportJob()
    {
        if (!reportflag.Contains("肚子饿了")&&hungerValue < 50)
        {
            userInterfaceController.ActivateReportDialogue("肚子饿了");
            reportflag.Add("肚子饿了");
        }
        if (reportflag.Contains("肚子饿了") && hungerValue > 80)
        {
            reportflag.Remove("肚子饿了");
        }
    }

    //食物搜寻播报
    public void FoodSearchReportJob()
    {
        if (!reportflag.Contains("附近有池塘，也许可以抓一些鱼来充饥")
            && EnterArea.Contains("池塘附近判定区域")
            && hungerValue < 50)
        {
            userInterfaceController.ActivateReportDialogue("附近有池塘，也许可以抓一些鱼来充饥");
            reportflag.Add("附近有池塘，也许可以抓一些鱼来充饥");
        }

        if (reportflag.Contains("附近有池塘，也许可以抓一些鱼来充饥")
            && !EnterArea.Contains("池塘附近判定区域"))
        {
            reportflag.Remove("附近有池塘，也许可以抓一些鱼来充饥");
        }
    }

    public void CharacterPropertyJob()
    {
        HungerValueRateJob();
        HungerReportJob();
        FoodSearchReportJob();
    }
    #endregion

    #region 角色工作模块

    public OriginWorkBubble workBubblePrefab;
    public Transform workBubbleParent;
    public string currentWork;
    public float currentWorkRate;
    private HashSet<string> workMap = new HashSet<string>();
    private HashSet<string> recordArea = new HashSet<string>();

    //显示工作气泡循环
    private void DisplayWorkBubbleJob()
    {
        //recordArea = new HashSet<string>(EnterArea);
        //print(!(recordArea.Count == EnterArea.Count && recordArea.IsSubsetOf(EnterArea)) && workMap.Overlaps(EnterArea));
        if (!(recordArea.Count == EnterArea.Count && recordArea.IsSubsetOf(EnterArea)) && workMap.Overlaps(EnterArea))
        {

            HashSet<string> areaIntersect = new HashSet<string>(workMap);
            HashSet<string> workSet = new HashSet<string>();
            areaIntersect.IntersectWith(EnterArea);
            recordArea = new HashSet<string>(EnterArea);

            foreach (var item in areaIntersect)
            {
                print(item);
                if (characterResource.areaToWorkLib.ContainsKey(item))
                {
                    workSet.UnionWith(characterResource.areaToWorkLib[item]);
                    print(workSet.Count);
                }
            }
            int index = 0;
            foreach (var item in workSet)
            {
                OriginWorkBubble obj = Instantiate(workBubblePrefab, workBubbleParent);
                obj.SetContent(characterResource.workTextureLib[item]);
                obj.transform.localPosition = new Vector3(index * 2 - workSet.Count / 2, 4.6f, 0);
                obj.name = item;
                obj.originCharacter = this;
                index++;
            }
            
        }


        if (workBubbleParent.childCount != 0
            &&Input.GetMouseButtonDown(0)
            && originRaySystem.clickName=="摸鱼")
        {
            eventLib.currentWorkString = "摸鱼";
        }

    }

    private void CharacterWorkMapInitialize()
    {
        workMap.Add("池塘区域");
    }

    private void CharacterWorkInitialize()
    {
        CharacterWorkMapInitialize();
    }

    private void CharacterWorkJob()
    {
        DisplayWorkBubbleJob();
    }
    #endregion
}

