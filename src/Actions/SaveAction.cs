// Copyright 2025 Woohyun Shin (sinusinu)
// SPDX-License-Identifier: GPL-3.0-only

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using SkiaSharp;

namespace Imago;

public class SaveAction : IAction {
    public static string Identifier => "save";

    Dictionary<string, string> vars = null!;

    private string path = null!;
    private string format = null!;
    private int quality = 100;

    private string[] supportedFormats = [ "bmp", "jpg", "gif", "png", "webp" ];
    private SKEncodedImageFormat[] skFormats = [ SKEncodedImageFormat.Bmp, SKEncodedImageFormat.Jpeg, SKEncodedImageFormat.Gif, SKEncodedImageFormat.Png, SKEncodedImageFormat.Webp ];

    public void Configure(Dictionary<string, string> options, Dictionary<string, string> vars) {
        if (!options.ContainsKey("format")) throw new Exception("Format is not provided");
        if (!supportedFormats.Contains(options["format"].ToLower(CultureInfo.InvariantCulture))) throw new Exception($"Format {options["format"]} is not supported");
        format = options["format"].ToLower(CultureInfo.InvariantCulture);
        
        if (options.ContainsKey("quality")) quality = int.Parse(options["quality"]);

        this.vars = vars;
    }

    public void Invoke(DisposableObjectHandler objectHandler) {
        var fi = -1;
        for (int i = 0; i < supportedFormats.Length; i++) if (supportedFormats[i] == format) fi = i;
        SKEncodedImageFormat skFormat = skFormats[fi];

        var bitmap = (SKBitmap)objectHandler["image"];
        var data = bitmap.Encode(skFormat, quality);
        File.WriteAllBytes(vars["full_path_no_ext"] + "." + supportedFormats[fi], data.AsSpan());
    }
}