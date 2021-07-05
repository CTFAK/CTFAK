package Movements;

import Services.CFile;

public class CMoveDefMouse extends CMoveDef
{
    public int mmDx;      				
    public int mmFx;
    public int mmDy;
    public int mmFy;
    public int mmFlags;

	public CMoveDefMouse()
	{
	}
	
	@Override
    public void load(CFile file, int length)
    {
        mmDx=file.readAShort();
        mmFx=file.readAShort();
        mmDy=file.readAShort();
        mmFy=file.readAShort();
        mmFlags=file.readAShort();
    }
}
