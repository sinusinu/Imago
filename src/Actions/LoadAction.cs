// Copyright 2025 Woohyun Shin (sinusinu)
// SPDX-License-Identifier: GPL-3.0-only

using System;
using System.Collections.Generic;
using SkiaSharp;

namespace Imago;

public class LoadAction : IAction {
    public static string Identifier => "load";

    private string path = null!;

    public void Configure(Dictionary<string, string> options, Dictionary<string, string> vars) {
        path = vars["full_path"];
    }

    public void Invoke(DisposableObjectHandler objectHandler) {
        if (objectHandler.Contains("image")) throw new Exception("Image is already loaded!");

        var img = SKBitmap.Decode(path);
        objectHandler["image"] = img;
    }
}