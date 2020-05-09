using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor.Build.Content;
using UnityEditor.iOS.Extensions.Common;
using UnityEngine;
using UnityEngine.SocialPlatforms;
[RequireComponent(typeof(UserInfo))]
public class UserController : MonoBehaviour
{
	/// <summary>
	/// 用户状态定义
	/// </summary>
	public enum UserStateType
	{
		Run,
		FindBar,
		Charging,
		getCharged
	}
	/// <summary>
	/// 用户状态
	/// </summary>
	[Header("初始状态")]
	public UserStateType state;

	/**********************************************************/
	//寻路
	[Header("寻路目标设置")]

	[Tooltip("Run的目标列表")]
	public Transform[] runTargets;

	[Tooltip("充电桩A")]
	public Transform barA;

	[Tooltip("充电桩B")]
	public Transform barB;
	/// <summary>
	/// 当前目标
	/// </summary>
	private Transform curTarget;
	/// <summary>
	/// 下一目标
	/// </summary>
	private Transform nextTarget;
	/// <summary>
	/// 开始绕圈
	/// </summary>
	public void goRotation()
	{

	}
	/// <summary>
	/// 持续绕圈
	/// </summary>
	public void doRotate()
	{

	}

	/**********************************************************/

	private void Start()
	{

	}
	private void Update()
	{
		StateMachine();
	}

	/**********************************************************/

	void StateMachine()
	{
		switch (state)
		{
			case UserStateType.Run:
				break;
			case UserStateType.FindBar:
				break;
			case UserStateType.Charging:
				break;
			case UserStateType.getCharged:
				break;
			default:
				break;
		}
	}


}

public abstract class UserState
{
	public abstract UserController.UserStateType StateType { get; }

	public abstract void Enter(params object[] param);

	public abstract void Update();

	public abstract void Exit();
}

public class UserRunState : UserState
{
	public override UserController.UserStateType StateType
	{
		get
		{
			return UserController.UserStateType.Run;
		}
	}

	public override void Enter(params object[] param)
	{
		userController = param[0] as UserController;
	}

	public override void Exit()
	{
		
	}

	public override void Update()
	{
		userController.doRotate();
	}

	/**********************************************************/

	private UserController userController;
}
