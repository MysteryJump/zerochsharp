#r "Microsoft.CSharp"
#load "generateText.csx"
using System;
using System.Text.RegularExpressions;

var regex = new Regex(Regex.Escape("!omikuji"));
Response.Name = regex.Replace(Response.Name, GenerateRandomedOmikujiText(), 1);
