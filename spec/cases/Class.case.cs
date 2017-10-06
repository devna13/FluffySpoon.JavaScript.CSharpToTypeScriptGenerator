﻿class BeforeClass
{
	public string MyProperty { get; set; }

	public string foo;
}

class MainClass<Foo> where Foo : new()
{
	public string MyProperty { get; set; }

	public SomeStuff<OtherStuff, RegularStuff> BlahProperty { get; set; }

	public List<OtherStuff> OtherBlahProperty { get; set; }

	public class SubClass
	{
		public string MyProperty { get; set; }
	}
}

class OtherClass
{
	public string MyProperty { get; set; }
}