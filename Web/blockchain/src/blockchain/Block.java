package blockchain;

import java.sql.Timestamp;

public class Block {

	public int iIndex;              //����
	public String sProof;           //������֤����������������棬��ʵ����һ��������֤����ȷ��ֵ��
	public String sPreviousHash;    //ǰһ�������Hashֵ
	public Timestamp tsCreateTime;  //���鴴��ʱ���  
	
	
	/*���ݿ�
	 * 
	 * �û�ÿ��ֵ��������õ�ϵͳ10ԪǮ�Ľ�����ͬʱ��Ӯ��ǰ��һ���û���2ԪǮ
	 * ������ͬʱ��Ҫ��¼�Լ���id����һ����ֵ�ߵ�id
	 * 
	 * */
	public String sSender;           //��һ����ֵ�ߵ�id
	public String sRecipient;        //��ǰ��ֵ�ߵ�id
	public final int iMoneyAward=10; //ϵͳ����������̶�
	public final int iMoneyWin=2;    //Ӯȡ����������̶�
	
	
	public Block(int index,String proof,String hash,Timestamp createtime,String sender,String recipient){
		iIndex=index;
		sProof=proof;
		sPreviousHash=hash;
		tsCreateTime=createtime;
		sSender=sender;
		sRecipient=recipient;
	}
	
	public String toInfoString(){
		return this.iIndex+"#"+this.sProof+"#"+(this.sPreviousHash==""?"*":this.sPreviousHash)+"#"+this.tsCreateTime+"#"+(this.sSender==""?"*":this.sSender)+"#"+(this.sRecipient==""?"*":this.sRecipient);
	}
}
