<%@ page language="java" contentType="text/html; charset=gb2312"
    pageEncoding="gb2312"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=gb2312">
<title>区块链充电桩，数据初始化...</title>

<script type="text/javascript"> 
function disptime(){
 var today = new Date(); //获得当前时间
 var hh = today.getHours();  //获得小时、分钟、秒
 var mm = today.getMinutes();
 var ss = today.getSeconds();
 /*设置div的内容为当前时间*/
 document.getElementById("myclock").innerHTML="现在时间:"+hh+":"+mm+":"+ss;
/*
  使用setTimeout在函数disptime()体内再次调用setTimeout
  设置定时器每隔1秒（1000毫秒），调用函数disptime()执行，刷新时钟显示
*/
  var myTime=setTimeout("disptime()",500);
}

function disptimes(){
	 var today = new Date(); //获得当前时间
	 var hh = today.getHours();  //获得小时、分钟、秒
	 var mm = today.getMinutes();
	 var ss = today.getSeconds();
	 /*设置div的内容为当前时间*/
	 document.getElementById("myclocks").innerHTML="开始时间:"+hh+":"+mm+":"+ss;
	}
	
function InitData(){
	
	var xmlhttp;
	if (window.XMLHttpRequest)
	{// code for IE7+, Firefox, Chrome, Opera, Safari
	  xmlhttp=new XMLHttpRequest();
	}
	else
	{// code for IE6, IE5
	  xmlhttp=new ActiveXObject("Microsoft.XMLHTTP");
	}

	xmlhttp.onreadystatechange=function()
	{
		if (xmlhttp.readyState==4 && xmlhttp.status==200)
		{
			//alert("|"+xmlhttp.responseText+"|");
			  if(xmlhttp.responseText=='0'){
				  	alert('数据初始化完成!');
				  	document.getElementById("isLoading").value="no";
					document.getElementById("subbtn").disabled="";
			  }else{
			  		alert('数据初始化出错!');
			  }
		}
	}
	
	
	xmlhttp.open("GET","/blockchain/init.jsp?t="+new Date().getTime(),true);
	xmlhttp.setRequestHeader("Content-type","application/x-www-form-urlencoded");
	xmlhttp.send();
	
}	

function checkinput(obj){
	if(obj.isLoading.value=='yes'){
		alert('正在同步区块数据，完成区块链数据同步后才可以进行充电，请稍候。');
		return false;
	}
	if(obj.mobile.value=='' || obj.mobile.value.length!=11 || isNaN(obj.mobile.value)){
		alert('分数会记在使用id下面，请正确输入你的使用id!');
		return false;
	}
	return true;
}
</script>
</head>
<body onload="disptime()">


<% java.util.Date d = new java.util.Date(); %>
<h1>
欢迎使用区块链充电桩。 <br>
</h1>

请稍等，完成区块链数据同步后才可以开始对充电桩充电。<br>
<br>
数据同步中...<br>
<br>
<div id="myclocks"></div>
<br>
<div id="myclock"></div>

<br>
<form action="/blockchain/detail.jsp" method="post" onsubmit="return checkinput(this);">
	<input type="hidden" name="isLoading" id="isLoading" value="yes">
	你的使用id是：<input type="text" name="mobile">&nbsp;<input type="submit" id="subbtn" value="开始使用" disabled="disabled">
</form>

<script type="text/javascript">
disptimes();
InitData();
</script>

<br>
-----------------------------------------------------------------------------------<br>

</body>
</html>