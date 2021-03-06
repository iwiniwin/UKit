﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using static UKit.Utils.Output;

/// <summary>
/// 统一事件管理
/// 3D事件如果使用，需要给主摄像机绑定 Physics Raycaster 组件
/// </summary>
public class EventListener : EventTrigger
{
    public UnityAction<GameObject, PointerEventData> onClick;
    public UnityAction<GameObject, PointerEventData> onEnter;
    public UnityAction<GameObject, PointerEventData> onExit;

    // 是否透传事件
    public bool passthrough = false;

    public override void OnPointerClick(PointerEventData eventData){
        base.OnPointerClick(eventData);
        if(onClick != null){
            onClick(gameObject, eventData);
        }
        PassEvent(eventData, ExecuteEvents.pointerClickHandler);
    }

    public override void OnPointerEnter(PointerEventData eventData){
        base.OnPointerEnter(eventData);
        if(onEnter != null){
            onEnter(gameObject, eventData);
        }
        PassEvent(eventData, ExecuteEvents.pointerEnterHandler);
    }

    public override void OnPointerExit(PointerEventData eventData){
        base.OnPointerExit(eventData);
        if(onExit != null){
            onExit(gameObject, eventData);
        }
        PassEvent(eventData, ExecuteEvents.pointerExitHandler);
    }

    /// <summary>
    /// 获取或者添加EventListener脚本来实现对游戏对象的监听
    /// </summary>
    public static EventListener Get(GameObject go, bool? passthrough = null){
        EventListener listener = go.GetComponent<EventListener>();
        if(listener == null){
            listener = go.AddComponent<EventListener>();
        }
        if(passthrough != null){
            listener.passthrough = (bool)passthrough;
        }
        return listener;
    }

    /// <summary>
    /// 渗透事件
    /// </summary>
    public void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function) 
        where T : IEventSystemHandler
    {
        if(!passthrough){
            return;
        }

        List<RaycastResult> results = new List<RaycastResult>();
        // 找到所有能够传递事件的对象
        EventSystem.current.RaycastAll(data, results);
        GameObject current = data.pointerCurrentRaycast.gameObject;
        for (int i = 0; i < results.Count; i++)
        {
            if(current != results[i].gameObject){
                ExecuteEvents.Execute(results[i].gameObject, data, function);
            }
        }
    }
}
