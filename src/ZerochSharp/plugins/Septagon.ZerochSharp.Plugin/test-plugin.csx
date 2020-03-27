#r "Microsoft.CSharp"
using System;

Console.WriteLine(Thread.Title);
// Thread.Title += "Nanamin";

Thread.Title += PluginSetting["AddText"];

