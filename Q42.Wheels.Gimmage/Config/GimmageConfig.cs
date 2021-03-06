﻿using System.Configuration;

namespace Q42.Wheels.Gimmage.Config
{
  internal static class GimmageConfig
  {
    private static GimmageConfigurationSection config = null;

    public static GimmageConfigurationSection Config
    {
      get { return config ?? (config = ConfigurationManager.GetSection("gimmage") as GimmageConfigurationSection); }
    }
  }

}