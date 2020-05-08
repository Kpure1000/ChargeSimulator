package blockchain;


import blockchain.Block;

import java.sql.Timestamp;
import java.util.*;
import java.io.*;
import java.net.*;

public class BlockChain {
	public static List<Block> lBlockchain=new ArrayList<>();    //用来存储区块
		
	
	
	public BlockChain(){
		//lBlockchain.add(null);
	}
	
	
	
	static final String  sIPPre="192.168.0.106";                 //对局域网内的电脑进行扫描，找到最长的链，下载到本地
	static final String  sDataFileDir="c://blockchain";     //本地存储路径
	
	//共识算法
	//返回 true 表示当前节点工作可以被认可
	public static boolean Unanimous(){
		boolean bRet=true;
		
		//扫描周边的节点，找到最长的链，下载到本地
		int iLastLen=0;
		String sLastChain="";
		for(int i=0;i<255;i+=1){
			String sThisURL="http://"+sIPPre+i+":8080/blockchain/chain.jsp";
			
			System.out.println(sThisURL);
			
			String sChain=httpRequest(sThisURL);
			
			if(sChain!=""){
				System.out.println(sChain);
				String sTemp[]=sChain.split("##");
				if(sTemp.length>iLastLen){
					iLastLen=sTemp.length;
					sLastChain=sChain;
				}
			}
		}
		
		BlockChain.LoadData();
		try{
			//如果其它节点存在长度大于本地节3点+1的链，则本次“挖矿”无效
			if(sLastChain!="" && iLastLen >= BlockChain.lBlockchain.size()+1){
				bRet=false;
				FileOutputStream out = new FileOutputStream(new File(sDataFileDir+"//data.txt"));
				sLastChain=sLastChain.replace("##", "\r\n");
				out.write((sLastChain+"\r\n").getBytes());
				out.close();
			}
		}catch(Exception e){	
		}
		return bRet;
	}
	
	
	//从网络读取区块链数据到本地文件
	public static void DowloadData(){

		//检查数据文件目录，不存在就创建
        File dirFile = new File(sDataFileDir);
        boolean bFile   = dirFile.exists();
		if(!bFile ){
			bFile = dirFile.mkdir();
			//往新创建的本地文件里面写一个创世块
			try{
				FileOutputStream out = new FileOutputStream(new File(dirFile+"//data.txt"));
				out.write((BlockChain.CreateFirstBlock().toInfoString()+"\r\n").getBytes());
				out.close();
			}catch(Exception e){	
			}
		}
		
		
		//扫描周边的节点，找到最长的链，下载到本地
		int iLastLen=0;
		String sLastChain="";
		for(int i=0;i<255;i+=1){
			String sThisURL="http://"+sIPPre+i+":8080/blockchain/chain.jsp";
			
			System.out.println(sThisURL);
			
			String sChain=httpRequest(sThisURL);
			
			if(sChain!=""){
				System.out.println(sChain);
				String sTemp[]=sChain.split("##");
				if(sTemp.length>iLastLen){
					iLastLen=sTemp.length;
					sLastChain=sChain;
				}
			}
		}
		
		try{
			if(sLastChain!=""){
				FileOutputStream out = new FileOutputStream(new File(dirFile+"//data.txt"));
				//System.out.println("before:"+sLastChain);
				sLastChain=sLastChain.replace("##", "\r\n");
				//System.out.println("after:"+sLastChain);
				out.write((sLastChain+"\r\n").getBytes());
				out.close();
			}
		}catch(Exception e){	
		}
		
	}
		
	//从文件读取区块链到内存
	public static void LoadData(){
		BlockChain.lBlockchain.clear();
		
		File file=new File(sDataFileDir+"//data.txt");  
        BufferedReader reader=null;  
        String temp=null;   
        try{  
                reader=new BufferedReader(new FileReader(file));  
                while((temp=reader.readLine())!=null){                  	
                	String [] sInfo=temp.split("#");
                	//System.out.println(sInfo[5]);
                	Timestamp tThis=new Timestamp((new java.text.SimpleDateFormat("yyyy-MM-dd HH:mm:ss")).parse(sInfo[3]).getTime());
                	Block bThis = BlockChain.NewBlock(Integer.parseInt(sInfo[0]), sInfo[1], (sInfo[2].equals("*")?"":sInfo[2]), tThis, (sInfo[4].equals("*")?"":sInfo[4]), (sInfo[5].equals("*")?"":sInfo[5]));
                	BlockChain.lBlockchain.add(bThis);
                }  
        }  
        catch(Exception e){  
            e.printStackTrace();  
        }  
        finally{  
            if(reader!=null){  
                try{  
                    reader.close();  
                }  
                catch(Exception e){  
                    //e.printStackTrace();  
                }  
            }  
        }  		
	}
	
	//把区块链从内存写到文件
	public static void WriteData(){
		
		String sChain="";
		
		int i=0;
		while(i<BlockChain.lBlockchain.size()){
			Block bThis=BlockChain.lBlockchain.get(i++);
			sChain+=(bThis.toInfoString()+"\r\n");
		}
		
		try{
				FileOutputStream out = new FileOutputStream(new File(sDataFileDir+"//data.txt"));
				out.write((sChain).getBytes());
				out.close();
		}catch(Exception e){	
		}		
	}
	
	public static String StringBlockchain(){
		String sRet="";
		
		int i=0;
		while(i<BlockChain.lBlockchain.size()){
			Block bThis=BlockChain.lBlockchain.get(i++);
			if(sRet=="")sRet=bThis.toInfoString();
			else sRet+="##"+bThis.toInfoString();
		}
		
		return sRet;
	}
	
	//创建新块
	public static Block NewBlock(int index,int proof,String hash,Timestamp createtime,String sender,String recipient){
		Block bRet=null;
		
		//在这里创建一个新块
		bRet = new Block(index,proof,hash,createtime,sender,recipient);
		
		return bRet;
	}
	
	//创始块的创建，创世块是一个块，必须是固定的信息
	//逻辑上来说，只有在区块链产品的第一个用户第一次启动的时候，才会需要创建创世块
	public static Block CreateFirstBlock(){
		try{
			Timestamp t=new Timestamp((new java.text.SimpleDateFormat("yyyy-MM-dd HH:mm:ss")).parse("2018-01-01 01:01:01").getTime());
			return NewBlock(0,"一号桩30%","",t,"","");
		}catch(Exception e){
			return null;
		}
	}
	
	
	//Hash 一个块
	public static String Hash(Block block){
		String sHash=null;
		
		//在这里Hash 一个块
		String s=block.sPreviousHash+block.sProof+block.sRecipient+block.sSender+block.tsCreateTime.toString();
		
		sHash = MD5(s);
	    
		return sHash;
	}
	
	public static String MD5(String key) {
        char hexDigits[] = {
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'
        };
        try {
            byte[] btInput = key.getBytes();
            // 获得MD5摘要算法的 MessageDigest 对象
            java.security.MessageDigest mdInst = java.security.MessageDigest.getInstance("MD5");
            // 使用指定的字节更新摘要
            mdInst.update(btInput);
            // 获得密文
            byte[] md = mdInst.digest();
            // 把密文转换成十六进制的字符串形式
            int j = md.length;
            char str[] = new char[j * 2];
            int k = 0;
            for (int i = 0; i < j; i++) {
                byte byte0 = md[i];
                str[k++] = hexDigits[byte0 >>> 4 & 0xf];
                str[k++] = hexDigits[byte0 & 0xf];
            }
            return new String(str);
        } catch (Exception e) {
            return null;
        }
    }	
	
	//验证当前的成语是否符合规则
	//pre 前一个工作量
	//cur 这一个工作量
	public static boolean ValidProof(String pre,String cur){
		
		//验证这个工作量是否从前一个工作量开始
		if(cur.charAt(0)!=pre.charAt(pre.length()-1)){

	         return false;

	      }
		
		//验证是否是成语
		//http://chengyu.t086.com/chaxun.php?q=%B9%E2%C3%F7%D5%FD%B4%F3&t=ChengYu
		String content=httpRequest("http://chengyu.t086.com/chaxun.php?q="+cur+"&t=ChengYu");
		if(content=="" || content.indexOf("没有找到与您您提交的数据")!=-1 || content.indexOf("搜索词太长")!=-1){
			return false;
		}
		
		return true;
		
	}
	

	
	
	//测试入口
	public static void main(String[] args) {
		System.out.println("区块链充电系统启动...");
		
		DowloadData();
		
		//BlockChain.LoadData();
		//System.out.println(BlockChain.StringBlockchain());
		
		//System.out.println(ValidProof("正大光明","明媒正娶"));
		
		
		/*
		try{
			System.out.println("本机的IP = " + java.net.InetAddress.getLocalHost());
		}catch(Exception e){
			
		}
		*/
		/*
		int x=5;
		int y=0;
		while(true){
			String md5=BlockChain.MD5(""+(x*y));
			System.out.println(x+"*"+y+"md5="+md5);
			if(md5.charAt(md5.length()-1)=='0' && md5.charAt(md5.length()-2)=='0'){
				break;
			}
			y+=1;
		}
		*/
		//System.out.println();
	}
	
	public static String httpRequest(String url){
		String content = "";
		HttpURLConnection connection = null;
		try{
			URL u = new URL(url);
			connection = (HttpURLConnection)u.openConnection();
			

			connection.setConnectTimeout(200);
			//connection.setReadTimeout(2000);
			
			connection.setRequestMethod("GET");

			int code = connection.getResponseCode();

			if(code == 200){
				InputStream in = connection.getInputStream();
				InputStreamReader isr = new InputStreamReader(in,"gb2312");
				BufferedReader reader = new BufferedReader(isr);
				String line = null;
				while((line = reader.readLine()) != null){
					content += line;
				}
			}

		}catch(MalformedURLException e){
			//e.printStackTrace();
		}catch(IOException e){
			//e.printStackTrace();
		}finally{
			if(connection != null){
				connection.disconnect();
			}
		}
		return content;
	
	}	
	
}
