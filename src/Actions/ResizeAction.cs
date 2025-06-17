// Copyright 2025 Woohyun Shin (sinusinu)
// SPDX-License-Identifier: GPL-3.0-only

using System;
using System.Collections.Generic;
using SkiaSharp;

namespace Imago;

public class ResizeAction : IAction {
    public static string Identifier => "resize";

    int? targetWidth = null;
    int? targetHeight = null;
    bool filter = true;

    public void Configure(Dictionary<string, string> options, Dictionary<string, string> vars) {
        if (options.ContainsKey("width")) {
            targetWidth = int.Parse(options["width"]);
            if (targetWidth < 1) throw new Exception("Width must be positive");
        }
        if (options.ContainsKey("height")) {
            targetHeight = int.Parse(options["height"]);
            if (targetHeight < 1) throw new Exception("Height must be positive");
        }

        if (targetWidth is null && targetHeight is null) {
            throw new Exception("Either one of width or height must be set");
        }

        if (options.ContainsKey("filter")) {
            filter = bool.Parse(options["filter"]);
        }
    }

    public void Invoke(DisposableObjectHandler objectHandler) {
        if (!objectHandler.Contains("image")) throw new Exception("Image is not loaded!");

        var bitmap = (SKBitmap)objectHandler["image"];
        SKBitmap resizedBitmap = null!;

        SKFilterMode skFilter = filter ? SKFilterMode.Linear : SKFilterMode.Nearest;

        if (targetWidth is not null && targetHeight is not null) {
            // both width and height set, ignore aspect ratio
            resizedBitmap = bitmap.Resize(new SKSizeI((int)targetWidth, (int)targetHeight), new SKSamplingOptions(skFilter));
        } else if (targetWidth is not null && targetHeight is null) {
            // keep aspect ratio, resize along x axis
            float hpw = bitmap.Height / (float)bitmap.Width;
            targetHeight = (int)Math.Round((int)targetWidth * hpw);
            resizedBitmap = bitmap.Resize(new SKSizeI((int)targetWidth, (int)targetHeight), new SKSamplingOptions(skFilter));
        } else if (targetWidth is null && targetHeight is not null) {
            // keep aspect ratio, resize along y axis
            float wph = bitmap.Width / (float)bitmap.Height;
            targetWidth = (int)Math.Round((int)targetHeight * wph);
            resizedBitmap = bitmap.Resize(new SKSizeI((int)targetWidth, (int)targetHeight), new SKSamplingOptions(skFilter));
        }

        objectHandler["image"] = resizedBitmap;
        bitmap.Dispose();
    }
}