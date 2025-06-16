// Copyright 2025 Woohyun Shin (sinusinu)
// SPDX-License-Identifier: GPL-3.0-only

using System;
using System.Collections.Generic;

namespace Imago;

public class DisposableObjectHandler : IDisposable {
    private Dictionary<string, IDisposable> disposableObjects;

    public DisposableObjectHandler() {
        disposableObjects = new();
    }

    public void Add(string key, IDisposable obj) {
        if (disposableObjects.ContainsKey(key)) disposableObjects.Remove(key);
        disposableObjects.Add(key, obj);
    }

    public bool Contains(string key) {
        return disposableObjects.ContainsKey(key);
    }

    public IDisposable this[string key] {
        get { return disposableObjects[key]; }
        set { Add(key, value); }
    }

    public void Dispose() {
        foreach (var i in disposableObjects.Values) i.Dispose();
    }
}