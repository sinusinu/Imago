// Copyright 2025 Woohyun Shin (sinusinu)
// SPDX-License-Identifier: GPL-3.0-only

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SkiaSharp;

namespace Imago;

public class RotateAction : IAction {
    public static string Identifier => "rotate";

    string targetAngle = "";
    string[] supportedAngles = [ "90", "180", "270" ];

    public void Configure(Dictionary<string, string> options, Dictionary<string, string> vars) {
        if (!options.ContainsKey("angle")) throw new Exception("Angle must be given");
        targetAngle = options["angle"];
        if (!supportedAngles.Contains(targetAngle)) throw new Exception("Angle must be one of: 90, 180, 270");
    }

    public void Invoke(DisposableObjectHandler objectHandler) {
        if (!objectHandler.Contains("image")) throw new Exception("Image is not loaded!");

        var bitmap = (SKBitmap)objectHandler["image"];
        SKBitmap rotatedBitmap = null!;
        
        if (targetAngle == "90") {
            rotatedBitmap = new SKBitmap(bitmap.Height, bitmap.Width);
            using (var cv = new SKCanvas(rotatedBitmap)) {
                cv.Translate(rotatedBitmap.Width, 0);
                cv.RotateDegrees(90f);
                cv.DrawBitmap(bitmap, 0, 0);
            }
        } else if (targetAngle == "180") {
            rotatedBitmap = new SKBitmap(bitmap.Width, bitmap.Height);
            using (var cv = new SKCanvas(rotatedBitmap)) {
                cv.Translate(rotatedBitmap.Width, rotatedBitmap.Height);
                cv.RotateDegrees(180f);
                cv.DrawBitmap(bitmap, 0, 0);
            }
        } else if (targetAngle == "270") {
            rotatedBitmap = new SKBitmap(bitmap.Height, bitmap.Width);
            using (var cv = new SKCanvas(rotatedBitmap)) {
                cv.Translate(0, rotatedBitmap.Height);
                cv.RotateDegrees(270f);
                cv.DrawBitmap(bitmap, 0, 0);
            }
        }

        objectHandler["image"] = rotatedBitmap;
        bitmap.Dispose();
    }
}