
# SCPM914-


[![Downloads](https://img.shields.io/github/downloads/YF-OFFICE/SCPM914-/total?color=brown&label=Downloads&style=for-the-badge)](https://github.com/YF-OFFICE/SCPM914-/releases)


功能:拾取物品概率升级 

~~~~
  public bool IsEnabled { get; set; } = true;
  public bool Debug { get; set; } = false;
  [Description("冷却四件")]
  public int Luck { get; set; } = 60;
  [Description("刷新几率")]
  public int Luck1 { get; set; } = 40;
  [Description("最大血量")]
  public int Health { get; set; } = 150;
  [Description("多少人时会刷新")]
  public int People { get; set; } = 10;
  [Description("m914开局默认给的物品")]
  public List<ItemType> itemTypes { get; set; } = new List<ItemType>() { };
 
~~~~


HAHAHHA
