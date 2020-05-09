using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UserInfo : MonoBehaviour
{
	/// <summary>
	/// 用户信息定义
	/// </summary>
	[Serializable]
	public struct Info
	{
		/// <summary>
		/// 电量信息
		/// </summary>
		[Range(0,maxBattery)]
		public int curBattery;

		public const int maxBattery = 500;

		public const int alarmBattery = 100;
		/// <summary>
		/// 余额
		/// </summary>
		[Tooltip("余额")]
		public float Balance;
		/// <summary>
		/// 充电币
		/// </summary>
		[Tooltip("充电币")]
		public int Currency;
	}
	/// <summary>
	/// 用户信息
	/// </summary>
	[Header("用户信息")]
	public Info userInfo;

	/**********************************************************/

	/// <summary>
	/// 充电记录
	/// </summary>
	public struct chargeRecord
	{
		public int powerModify;
		public float balanceModify;
		public int rewardsModify;
	}
	/// <summary>
	/// 充电记录表
	/// </summary>
	[NonSerialized]
	public List<chargeRecord> chargeRecords;

	/**********************************************************/

	private void Start()
	{
		
	}
}
