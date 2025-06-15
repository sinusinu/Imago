// Copyright 2025 Woohyun Shin (sinusinu)
// SPDX-License-Identifier: GPL-3.0-only

using System.Collections.Generic;

namespace Imago;

public interface IAction {
    public static string? Identifier { get; }
    public void Configure(Dictionary<string, string> options, Dictionary<string, string> vars);
    public void Invoke(DisposableObjectHandler objectHandler);
}