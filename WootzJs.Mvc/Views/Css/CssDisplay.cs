﻿#region License

//-----------------------------------------------------------------------
// <copyright>
// The MIT License (MIT)
// 
// Copyright (c) 2014 Kirk S Woll
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
//-----------------------------------------------------------------------

#endregion

using System;

namespace WootzJs.Mvc.Views.Css
{
    public enum CssDisplay
    {
        Inherit, None, Block, InlineBlock
    }

    public static class CssDisplays
    {
        public static CssDisplay Parse(string s)
        {
            switch (s)
            {
                case "inherit":
                    return CssDisplay.Inherit;
                case "none":
                    return CssDisplay.None;
                case "block":
                    return CssDisplay.Block;
                case "inline-block":
                    return CssDisplay.InlineBlock;
                default:
                    throw new Exception();
            }
        }

        public static string GetCssValue(this CssDisplay value)
        {
            switch (value)
            {
                case CssDisplay.Inherit:
                    return "inherit";
                case CssDisplay.None:
                    return "none";
                case CssDisplay.Block:
                    return "block";
                case CssDisplay.InlineBlock:
                    return "inline-block";
                default:
                    throw new Exception();
            }
        }
    }
}