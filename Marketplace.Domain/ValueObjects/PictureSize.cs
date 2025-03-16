﻿using Marketplace.Framework.Common;

namespace Marketplace.Domain.ValueObjects;

public class PictureSize : Value<PictureSize>
{
    public int Width { get; internal set; }
    public int Height { get; internal set; }

    public PictureSize(int width, int height)
    {
        if (Width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than 0.");
        }

        if (Height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height), "Height must be greater than 0.");
        }

        Width = width;
        Height = height;
    }
    
    internal PictureSize() { }
}