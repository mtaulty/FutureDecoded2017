﻿using App1;
using System;
using Windows.ApplicationModel.Core;

namespace HolographicApp1
{
  /// <summary>
  /// Windows Holographic application using SharpDX.
  /// </summary>
  internal class Program
  {
    /// <summary>
    /// Defines the entry point of the application.
    /// </summary>
    [MTAThread]
    private static void Main()
    {
      Windows.UI.Xaml.Application.Start(p => new App());
    }
  }
}