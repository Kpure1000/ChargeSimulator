
<%@page import="blockchain.BlockChain"%>
<%@ page language="java" contentType="text/html; charset=gb2312"
    pageEncoding="gb2312"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=gb2312">
<title>区块链充电</title>

<script type="text/javascript"> 

var iReload=0

function disptime(){
	iReload+=1;
	
	
	if(iReload>60*10 && document.getElementById("isLoading").value=="no"){ //10分钟，重新刷新一下区块链数据
		InitData();
		iReload=0;
	}
	
	
	 var today = new Date(); //获得当前时间
	 var hh = today.getHours();  //获得小时、分钟、秒
	 var mm = today.getMinutes();
	 var ss = today.getSeconds();
	 /*设置div的内容为当前时间*/

	 
	 if(document.getElementById("isLoading").value=="no"){
		 document.getElementById("myclock").innerHTML="现在时间:"+hh+":"+mm+":"+ss+"<br>本地区块链数据为最新。";
	 }else{
		 document.getElementById("myclock").innerHTML="现在时间:"+hh+":"+mm+":"+ss+"<br>正在检测和更新本地区块链数据。。。";
	 }
	 
	/*
	  使用setTimeout在函数disptime()体内再次调用setTimeout
	  设置定时器每隔1秒（1000毫秒），调用函数disptime()执行，刷新时钟显示
	*/
	  var myTime=setTimeout("disptime()",1000);
}

	
function InitData(){
	document.getElementById("isLoading").value="yes";
	
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

			  if(xmlhttp.responseText=='0'){
				  	//alert('数据初始化完成!');
				  	document.getElementById("isLoading").value="no";
					//document.getElementById("subbtn").disabled="";
			  }else{
			  		//alert('数据初始化出错!');
			  }
		}
	}
	
	
	xmlhttp.open("GET","/blockchain/init.jsp?t="+new Date().getTime(),true);
	xmlhttp.setRequestHeader("Content-type","application/x-www-form-urlencoded");
	xmlhttp.send();
	
}	

function CheckData(){
	document.getElementById("isLoading").value="yes";
	
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

			  document.getElementById("isLoading").value="no";
			  if(xmlhttp.responseText=='true'){
				  	document.getElementById("subdirect").value="yes";
				 	 document.getElementById("answer").disabled=false;
				  	document.getElementById("subbtn").disabled=false;
				  	document.getElementById("subbtn").value="确定";	  				  
				  	document.getElementById("subform").submit();
			  }else{
			  		alert("你慢了一步！已经有其它用户完成了对该充电桩的充电！");
			  		window.location.reload(true);
				  	//document.getElementById("subdirect").value="yes";
				 	//document.getElementById("answer").disabled=false;
				  	//document.getElementById("subbtn").disabled=false;
				  	//document.getElementById("subbtn").value="确定";	  				  
				  	//document.getElementById("subform").submit();		  		
			  }
		}
	}
	
	
	xmlhttp.open("GET","/blockchain/check.jsp?t="+new Date().getTime(),true);
	xmlhttp.setRequestHeader("Content-type","application/x-www-form-urlencoded");
	xmlhttp.send();
	
}

function checkinput(obj){
	/*
	if(obj.isLoading.value=='yes'){
		alert('正在同步区块数据，完成区块链数据同步后才可以开始充电，请稍候。');
		return false;
	}
	*/
	if(document.getElementById("subdirect").value=="yes"){
		return true;
	}
	
	if(document.getElementById("subbtn").disabled==true){
		return false;
	}
	
	if(obj.answer.value==''){
		alert('请输入你要充值的电量!');
		return false;
	}
	
	CheckData();
	
	document.getElementById("subbtn").value="真在递交和检测你的数据，请稍等。。。";
	document.getElementById("subbtn").disabled=true;
	document.getElementById("answer").disabled=true;
	//document.getElementById("linfo").innerText="正在检测有效性...";
	
	return false;
}
</script>
</head>
<body onload="disptime();">




<%
if(request.getParameter("mobile")!=null){
	session.setAttribute("mobile", request.getParameter("mobile"));
}
%>


<%
if(request.getParameter("answer")!=null){
	
	//加载区块链数据
	blockchain.BlockChain.LoadData();
	
	//获取区块链中最后一个成语
	String sPre=blockchain.BlockChain.lBlockchain.get(blockchain.BlockChain.lBlockchain.size()-1).sProof;
	
	//用户输入的成语
	String sCur=new String(request.getParameter("answer").getBytes("ISO8859-1"),"gb2312");
	
	System.out.println(sCur);
	
	//判断是不是正确答案，如果是，就要创建新块并添加到区块链中
	if(blockchain.BlockChain.ValidProof(sPre,sCur)){
		
		blockchain.Block bPre=blockchain.BlockChain.lBlockchain.get(blockchain.BlockChain.lBlockchain.size()-1);
		
		//获取前一个块的Hash
		String sHash=blockchain.BlockChain.Hash(bPre);
		
		//创建新块
		blockchain.Block bCur=blockchain.BlockChain.NewBlock(bPre.iIndex+1, sCur, sHash, new java.sql.Timestamp(System.currentTimeMillis()), bPre.sRecipient, (String)session.getAttribute("mobile"));
		
		//加入区块链并保存到本地文件
		blockchain.BlockChain.lBlockchain.add(bCur);
		blockchain.BlockChain.WriteData();
		%>
		<script type="text/javascript">alert('恭喜恭喜！充电完成！');</script>
		<%
	}else{
		%>
		<script type="text/javascript">alert('很抱歉，充电失败！');</script>
		<%
	}
}

%>

<h1>
欢迎使用区块链充电系统。 &nbsp;&nbsp;<a href="/blockchain/detail.jsp">刷新</a></a><br>
</h1>
<%
blockchain.BlockChain.LoadData();
int iScore=0;
int i=0;
while(i<blockchain.BlockChain.lBlockchain.size()){
	blockchain.Block bThis=blockchain.BlockChain.lBlockchain.get(i++);
	//System.out.println("/"+session.getAttribute("mobile")+"/"+bThis.sRecipient+"/");
	if(session.getAttribute("mobile").equals(bThis.sRecipient)){
		if(bThis.sSender!="")iScore+=(bThis.iMoneyAward+bThis.iMoneyWin);
		else iScore+=(bThis.iMoneyAward);
	}
	if(session.getAttribute("mobile").equals(bThis.sSender)){
		iScore-=bThis.iMoneyWin;
	}	
}
%>
<br>
你的使用ID是：<%= session.getAttribute("mobile") %><br>
<br>
你的充电币是：<%= iScore %><br>
<HR>

<%
String sProofs = "-><b>"+blockchain.BlockChain.lBlockchain.get(blockchain.BlockChain.lBlockchain.size()-1).sProof+"</b>";
if(blockchain.BlockChain.lBlockchain.size()>=2){
	sProofs = "->"+blockchain.BlockChain.lBlockchain.get(blockchain.BlockChain.lBlockchain.size()-2).sProof+sProofs;
}
if(blockchain.BlockChain.lBlockchain.size()>=3){
	sProofs = "->"+blockchain.BlockChain.lBlockchain.get(blockchain.BlockChain.lBlockchain.size()-3).sProof+sProofs;
}
if(blockchain.BlockChain.lBlockchain.size()>3){
	sProofs = "..."+sProofs;
}else{
	//sProofs = "》"+sProofs;
}
%>
当前等待充电的充电桩电量是：<%= sProofs %><br>
<br>
<form id="subform" action="/blockchain/detail.jsp" method="post" onsubmit="return checkinput(this);">
	<input type="hidden" name="isLoading" id="isLoading" value="no">
	<input type="hidden" name="isLoading" id="subdirect" value="no">
	请输入“<%=blockchain.BlockChain.lBlockchain.get(blockchain.BlockChain.lBlockchain.size()-1).sProof %>”充值电量：<input type="text" id="answer" name="answer">&nbsp;<input type="submit" id="subbtn" value="确定">&nbsp;<lable id="linfo"></lable>
</form>

<HR>
<div id="myclock"></div>
<br>

</body>
</html>