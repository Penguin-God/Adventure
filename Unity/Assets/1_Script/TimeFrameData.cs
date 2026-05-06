using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Proto/TimeFrame")]
public class TimeFrameData : ScriptableObject
{
    public string Id;                  // 유니크 ID
    public Sprite snapshotImage;       // 그 시점을 보여줄 이미지
    public int order;
}
