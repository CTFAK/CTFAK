package Movements;

import Objects.CObject;

public class CMoveMouse extends CMove
{
    public int MM_DXMouse;
    public int MM_DYMouse;
    public int MM_FXMouse;
    public int MM_FYMouse;
    public int MM_Stopped;
    public int MM_OldSpeed;


	public CMoveMouse()
	{
	}

	@Override
    public void init(CObject ho, CMoveDef mvPtr)
    {
        hoPtr=ho;
        
        CMoveDefMouse mmPtr= (CMoveDefMouse) mvPtr;
        
        hoPtr.roc.rcPlayer=mmPtr.mvControl;
		MM_DXMouse=mmPtr.mmDx+hoPtr.hoX;
		MM_DYMouse=mmPtr.mmDy+hoPtr.hoY;
		MM_FXMouse=mmPtr.mmFx+hoPtr.hoX;
		MM_FYMouse=mmPtr.mmFy+hoPtr.hoY;
		hoPtr.roc.rcSpeed=0;
		MM_OldSpeed=0;
		MM_Stopped=0;
		hoPtr.roc.rcMinSpeed=0;
		hoPtr.roc.rcMaxSpeed=100;
		rmOpt=mmPtr.mvOpt;
		moveAtStart(mvPtr);
		hoPtr.roc.rcChanged=true;
    }

	@Override
    public void move()
    {
		int newX=hoPtr.hoX;
		int newY=hoPtr.hoY;
		int deltaX, deltaY, flags, speed, dir, indexz;

		if (rmStopSpeed==0) 
        {
            if (hoPtr.hoAdRunHeader.rh2InputMask[hoPtr.roc.rcPlayer-1]!=0)      // no input?
            {
                newX=hoPtr.hoAdRunHeader.rhApp.mouseX;						//; Coordonnee en X
                if (newX<MM_DXMouse)
                    newX=MM_DXMouse;
                if (newX>MM_FXMouse)
                    newX=MM_FXMouse;

                newY=hoPtr.hoAdRunHeader.rhApp.mouseY;						//; Coordonnee en Y
                if (newY<MM_DYMouse)
                    newY=MM_DYMouse;
                if (newY>MM_FYMouse)
                    newY=MM_FYMouse;

                // Calcul de la pente du mouvement pour les animations
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                deltaX=newX-hoPtr.hoX;
                deltaY=newY-hoPtr.hoY;
                flags=0;							//; Flags de signe
                if (deltaX<0)							//; DX negatif?
                {
                    deltaX=-deltaX;
                    flags|=0x01;
                }
                if (deltaY<0)							//; DY negatif?
                {
                    deltaY=-deltaY;
                    flags|=0x02;
                }
                speed=(deltaX+deltaY)<<2;			//; Calcul de la vitesse (approximatif)
                if (speed>250) speed=250;
                hoPtr.roc.rcSpeed=speed;
                if (speed!=0) 
                {
                    deltaX<<=8;								//; * 256 pour plus de precision
                    if (deltaY==0) 
                        deltaY=1;
                    deltaX/=deltaY;
                    int index;
                    for (index=0; ; index+=2)
                    {
                        if (deltaX>=CosSurSin32[index]) 
                            break;
                    }		
                    dir=CosSurSin32[index+1];			//; Charge la direction
                    if ((flags&0x02)!=0)
                    {
                        dir=-dir+32;						//; Retablir en Y
                        dir&=31;
                    }
                    if ((flags&0x01)!=0)
                    {
                        dir-=8;								//; Retablir en X
                        dir&=31;
                        dir=-dir;
                        dir&=31;
                        dir+=8;
                        dir&=31;
                    }
                    hoPtr.roc.rcDir=dir;					//; Direction finale
                }
            }
        }
	
        // Appel des animations (temporise la vitesse)
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		if (hoPtr.roc.rcSpeed!=0)
		{
            MM_Stopped=0;
            MM_OldSpeed=hoPtr.roc.rcSpeed;
		}
		MM_Stopped++;
		if (MM_Stopped>10)
		{
            MM_OldSpeed=0;
		}
		hoPtr.roc.rcSpeed=MM_OldSpeed;
		if (hoPtr.roa!=null)
            hoPtr.roa.animate();;

		// Appel des collisions
		// ~~~~~~~~~~~~~~~~~~~~
		hoPtr.hoX=newX;					//; Les coordonnees
		hoPtr.hoY=newY;
		hoPtr.roc.rcChanged=true;
		hoPtr.hoAdRunHeader.rh3CollisionCount++;			//; Marque l'objet pour ce cycle
		rmCollisionCount=hoPtr.hoAdRunHeader.rh3CollisionCount;
		hoPtr.hoAdRunHeader.newHandle_Collisions(hoPtr);        
    }
	
	@Override
    public void stop()
    {
		// Pas de STOP si c'est le sprite courant... on contourne les obstacles
		if (rmCollisionCount==hoPtr.hoAdRunHeader.rh3CollisionCount)
		{
            mv_Approach((rmOpt&MVTOPT_8DIR_STICK)!=0);
		}
		hoPtr.roc.rcSpeed=0;
    }
    
    @Override
    public void start()
    {
		rmStopSpeed=0;
		hoPtr.rom.rmMoveFlag=true;
    }
    
    @Override
    public void setXPosition(int x)
    {        
		if (hoPtr.hoX!=x)
		{
		    hoPtr.hoX=x;
		    hoPtr.rom.rmMoveFlag=true;
		    hoPtr.roc.rcChanged=true;
		    hoPtr.roc.rcCheckCollides=true;					//; Force la detection de collision
		}
    }
    @Override
    public void setYPosition(int y)
    {
		if (hoPtr.hoY!=y)
		{
		    hoPtr.hoY=y;
		    hoPtr.rom.rmMoveFlag=true;
		    hoPtr.roc.rcChanged=true;
		    hoPtr.roc.rcCheckCollides=true;					//; Force la detection de collision
		}
    }

	@Override
	public void kill() {
	}

	@Override
	public void bounce() {
	}

	@Override
	public void reverse() {
	}

	@Override
	public void setSpeed(int speed) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void setMaxSpeed(int speed) {
	}

	@Override
	public void setDir(int dir) {
	}


}
