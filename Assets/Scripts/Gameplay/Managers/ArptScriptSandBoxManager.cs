#pragma warning disable IDE0004, IDE0051
using Arcript.Aspt.RawArcVNScripts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Enhance;
using System.Enhance.Unity;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using UnityEngine;

namespace Arcript.Gameplay
{
	public class ArptScriptSandBoxManager : Singleton<ArptScriptSandBoxManager>
	{
		private RawArcVNScript script;
		private ArcVNScriptCmdBase cmdCurrent;

		private Dictionary<string, int> varDict = new Dictionary<string, int>();

		protected override void SingletonAwake()
		{
			AllowRepeatInit = true;
		}

		public void Load(RawArcVNScript script)
		{
			this.script = script;
			cmdCurrent = !script.OutOfRange(0) ? script.Commands[0] : (ArcVNScriptCmdBase)null;
		}

		private object ExecuteExprFuncArg(FuncArgument funcArg)
		{
			var type = typeof(ArptScriptSandBoxManager);
			var methods = type.GetMethods()
				.Where(m => m.GetCustomAttributes(typeof(DescriptionAttribute), false).Length > 0)
				.ToArray();

			var methodDict = methods.ToDictionary(
				m => ((DescriptionAttribute)m.GetCustomAttributes(typeof(DescriptionAttribute), false)[0]).Description,
				m => m);

			if (methodDict.TryGetValue(funcArg.FuncName, out var method))
			{
				object r = method.Invoke(this, funcArg.Arguments.ToArray());
				return r;
			}
			else
			{
				return null;
			}
		}

		[Description("varCheck")]
		private bool VariableCheck(string exprStr)
		{
			// varCheck("myVar >= 5") => "myVar >= 5" 然后再解析
			// 将格式与上述一致的表达式解析后返回varDict["myVar"] >= 5的结果
			var @param = Expression.Parameter(typeof(Dictionary<string, int>), "varDict");
			var @expr = DynamicExpressionParser.ParseLambda(new[] { @param }, typeof(bool), exprStr);
			try
			{
				var compiled = @expr.Compile();
				return (bool)compiled.DynamicInvoke(varDict);
			}
			catch (KeyNotFoundException)
			{
				Debug.LogError($"varCheck: Variable {exprStr} not found. Did you use 'var {{varName}} = {{initValue}};' to define first?");
				return false;
			}
		}

		[Description("varSet")]
		private int VariableSet(params object[] exprArgs)
		{
			// varSet("myAnotherVar", 1495) => "myAnotherVar = 1495"* 然后再解析
			// *注: 实际传入的exprStr只有"myAnotherVar"和1495两个参数
			var @param = Expression.Parameter(typeof(Dictionary<string, int>), "varDict");
			var @expr = DynamicExpressionParser.ParseLambda(new[] { @param }, typeof(int), $"{exprArgs[0]} = {exprArgs[1]}");
			try
			{
				var compiled = @expr.Compile();
				return (int)compiled.DynamicInvoke(varDict);
			}
			catch (KeyNotFoundException)
			{
				Debug.LogError($"varSet: Variable {exprArgs[0]} not found. Did you use 'var {{varName}} = {{initValue}};' to define first?");
				return 0;
			}
		}

		private void VariableDefine(params object[] exprArgs)
		{
			// var myVar = 5 => "int myVar = 5;"* 然后再解析(=> [实际效果]varDict.Add("myVar", 5); 或 varDict["myVar"] = 5;)
			// *注: 实际传入的exprStr只有"myVar"和5两个参数
			var @param = Expression.Parameter(typeof(Dictionary<string, int>), "varDict");
			var @expr = DynamicExpressionParser.ParseLambda(new[] { @param }, typeof(void), $"varDict.Add({exprArgs[0]}, {exprArgs[1]});");
			try
			{
				var compiled = @expr.Compile();
				compiled.DynamicInvoke(varDict);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning($"varDefine: Variable {exprArgs[0]} already defined. Don't duplicately define a variable. (Command ignored)");
			}
		}
	}
}
