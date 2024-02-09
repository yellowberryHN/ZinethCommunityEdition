using UnityEngine;

public struct Color32
{
    public byte r;
    public byte g;
    public byte b;
    public byte a;

    private Color Color
    {
        get { return new Color(r / 255f, g / 255f, b / 255f, a / 255f); }
        set
        {
            r = (byte)(value.r * 255);
            g = (byte)(value.g * 255);
            b = (byte)(value.b * 255);
            a = (byte)(value.a * 255);
        }
    }

    public int PackedColor
    {
        get
        {
            int value = a;
            value |= b << 8;
            value |= g << 16;
            value |= r << 24;

            return value;
        }
    }

    public Color32(int r, int g, int b, int a)
    {
        this.r = (byte)r;
        this.g = (byte)g;
        this.b = (byte)b;
        this.a = (byte)a;
    }
	
    public Color32(int r, int g, int b)
    {
        this.r = (byte)r;
        this.g = (byte)g;
        this.b = (byte)b;
        a = 255;
    }

    public Color32(int packed)
    {
        r = (byte)((packed >> 24) & 0xFF);
        g = (byte)((packed >> 16) & 0xFF);
        b = (byte)((packed >> 8) & 0xFF);
        a = (byte)(packed & 0xFF);
    }

    public static implicit operator Color (Color32 instance)
    {
        return instance.Color;
    }

    public static implicit operator Color32	(Color color)
    {
        return new Color32 { Color = color };
    }
}