using System;
using System.Text;
using System.Collections.Generic;

namespace jonson.meta {
    public enum MetaError {
        None,
        RootIsNotObject,
        DescriptionIsNotObject,
        FieldTypeIsNotString,
        NoTypeInfo,
        IsRefMustBeBool,
        MissingCommaInDictionary,
        MissingClosingInDictionary,
        UnsupportedPrimitive,
        SlaveInfoMustBeArray,
        SlaveMustBeString,
        SlaveMustBePrimitive
    }

    public struct MetaRes {
        public string code;
        public MetaError error;

        public static MetaRes Code(string code) {
            return new MetaRes { code = code };
        }

        public static MetaRes Err(MetaError error) {
            return new MetaRes { error = error };
        }
    }

    class IndentMaster {
        public static void Add(int indentCount, StringBuilder builder) {
            for (int i = 0; i < indentCount; i++) {
                builder.Append("    ");
            }
        }
    }

    class LayoutBuilder {
        private string namespaceName;
        private string classModifiers;
        private string className;
        private List<string> usings = new List<string>();

        public LayoutBuilder Using(string usingName) {
            usings.Add(usingName);
            return this;
        }

        public LayoutBuilder Namespace(string name) {
            this.namespaceName = name;
            return this;
        }

        public LayoutBuilder Class(string modifiers, string name) {
            this.classModifiers = modifiers;
            this.className = name;
            return this;
        }

        public LayoutBuilder BuildPreMethods(StringBuilder builder, ref int indent) {
            foreach (var usingName in usings) {
                builder.Append($"using {usingName};\n");
            }

            if (usings.Count > 0) {
                builder.Append('\n');
            }

            builder.Append($"namespace {namespaceName}");
            builder.Append(" {\n");

            indent++;

            IndentMaster.Add(indent, builder);

            builder.Append(classModifiers);
            builder.Append(" class ");
            builder.Append(className);
            builder.Append(" {\n");

            indent++;

            return this;
        }

        public LayoutBuilder BuildPostMethods(StringBuilder builder, ref int indent) {
            indent--;

            IndentMaster.Add(indent, builder);
            builder.Append("}\n");

            indent--;
            IndentMaster.Add(indent, builder);
            builder.Append("}");

            return this;
        }
    }

    class MethodBuilder {
        private string modifiers;
        private string returnType;
        private string name;
        private List<string> parameters = new List<string>();

        public MethodBuilder Modifiers(string modifiers) {
            this.modifiers = modifiers;
            return this;
        }

        public MethodBuilder Return(string returnType) {
            this.returnType = returnType;
            return this;
        }

        public MethodBuilder Name(string name) {
            this.name = name;
            return this;
        }

        public MethodBuilder Parameter(string param) {
            parameters.Add(param);
            return this;
        }

        public MethodBuilder BuildPreBody(StringBuilder builder, ref int indent) {
            IndentMaster.Add(indent, builder);

            builder.Append(modifiers);
            builder.Append(' ');
            builder.Append(returnType);
            builder.Append(' ');
            builder.Append(name);

            builder.Append('(');
            for (int i = 0; i < parameters.Count; i++) {
                builder.Append(parameters[i]);

                if (i < parameters.Count - 1) {
                    builder.Append(", ");
                }
            }
            builder.Append(')');
            builder.Append(" {\n");

            indent++;

            return this;
        }

        public MethodBuilder BuildPostBody(StringBuilder builder, ref int indent) {
            indent--;
            IndentMaster.Add(indent, builder);
            builder.Append("}\n");
            return this;
        }
    }

    class DeclareBuilder {
        private string type;
        private string name;
        private bool initialize;
        private string right;
        private string initParams;

        public DeclareBuilder Type(string type) {
            this.type = type;

            return this;
        }

        public DeclareBuilder Name(string name) {
            this.name = name;

            return this;
        }

        public DeclareBuilder Initialize(string initParams = null) {
            this.initialize = true;
            this.initParams = initParams;

            return this;
        }

        public DeclareBuilder Assign(string right) {
            this.right = right;

            return this;
        }

        public DeclareBuilder Build(StringBuilder builder, ref int indent) {
            IndentMaster.Add(indent, builder);

            builder.Append(type);
            builder.Append(' ');
            builder.Append(name);

            if (initialize) {
                builder.Append(" = new ");
                builder.Append(type);
                builder.Append('(');

                if (initParams != null) {
                    builder.Append(initParams);
                }

                builder.Append(')');
            }

            if (right != null) {
                builder.Append(" = ");
                builder.Append(right);
            }

            builder.Append(";\n");

            return this;
        }
    }

    class AssignBuilder {
        private string left;
        private string right;

        public AssignBuilder Left(string left) {
            this.left = left;

            return this;
        }

        public AssignBuilder Right(string right) {
            this.right = right;

            return this;
        }

        public AssignBuilder Build(StringBuilder builder, ref int indent) {
            IndentMaster.Add(indent, builder);

            builder.Append($"{left} = {right};\n");

            return this;
        }
    }

    class IfBuilder {
        private string condition;

        public IfBuilder Condition(string condition) {
            this.condition = condition;

            return this;
        }

        public IfBuilder BuildCondition(StringBuilder builder, ref int indent) {
            IndentMaster.Add(indent, builder);

            builder.Append("if (");
            builder.Append(condition);
            builder.Append(") {\n");

            indent++;

            return this;
        }

        public IfBuilder BuildElse(StringBuilder builder, ref int indent) {
            indent--;
            IndentMaster.Add(indent, builder);

            builder.Append("} else {\n");
            indent++;

            return this;
        }

        public IfBuilder BuildEndCondition(StringBuilder builder, ref int indent) {
            indent--;

            IndentMaster.Add(indent, builder);

            builder.Append('}');
            builder.Append('\n');

            return this;
        }
    }

    class ForBuilder {
        private string init;
        private string condition;
        private string increment;

        public ForBuilder Init(string init) {
            this.init = init;

            return this;
        }

        public ForBuilder Condition(string condition) {
            this.condition = condition;

            return this;
        }

        public ForBuilder Increment(string increment) {
            this.increment = increment;

            return this;
        }

        public ForBuilder BuildPre(StringBuilder builder, ref int indent) {
            IndentMaster.Add(indent, builder);

            builder.Append("for (");
            builder.Append(init);
            builder.Append("; ");
            builder.Append(condition);
            builder.Append("; ");
            builder.Append(increment);
            builder.Append(") {\n");

            indent++;

            return this;
        }

        public ForBuilder BuildPost(StringBuilder builder, ref int indent) {
            indent--;
            IndentMaster.Add(indent, builder);
            builder.Append("}\n");

            return this;
        }
    }

    class ForeachBuilder {
        private string variable;
        private string enumerable;

        public ForeachBuilder Variable(string variable) {
            this.variable = variable;
            return this;
        }

        public ForeachBuilder Enumerable(string enumerable) {
            this.enumerable = enumerable;
            return this;
        }

        public ForeachBuilder BuildPre(StringBuilder builder, ref int indent) {
            IndentMaster.Add(indent, builder);

            builder.Append("foreach (var ");
            builder.Append(variable);
            builder.Append(" in ");
            builder.Append(enumerable);
            builder.Append(") {\n");

            indent++;

            return this;
        }

        public ForeachBuilder BuildPost(StringBuilder builder, ref int indent) {
            indent--;
            IndentMaster.Add(indent, builder);
            builder.Append("}\n");

            return this;
        }
    }

    class ReturnBuilder {
        private string body;

        public ReturnBuilder Body(string body) {
            this.body = body;
            return this;
        }

        public ReturnBuilder Build(StringBuilder builder, ref int indent) {
            IndentMaster.Add(indent, builder);

            builder.Append("return");
            if (body != "") {
                builder.Append(" ");
                builder.Append(body);
            }
            builder.Append(";\n");

            return this;
        }
    }

    public static class Meta {
        public static MetaRes GenerateFromJSON(
            JSONType description,
            string mainNamespace,
            List<string> usings
        ) {
            if (description.Obj.IsNone()) {
                return MetaRes.Err(MetaError.RootIsNotObject);
            }

            var objs = description.Obj.Peel();

            LayoutBuilder layoutBuilder = new LayoutBuilder();
            StringBuilder builder = new StringBuilder();
            int indent = 0;

            layoutBuilder = layoutBuilder
            .Using("System.Collections.Generic")
            .Using("System.Globalization");

            for (int i = 0; i < usings.Count; i++) {
                layoutBuilder = layoutBuilder.Using(usings[i]);
            }

            layoutBuilder
            .Namespace(mainNamespace)
            .Class("public static", "FromJSONExtensions")
            .BuildPreMethods(builder, ref indent);

            var slaveTypes = new HashSet<string>();

            foreach (var typeDescPair in objs) {
                var descType = typeDescPair.Value;

                var typeName = typeDescPair.Key;

                if (descType.Obj.IsNone()) {
                    if (typeName == "__slaves") {
                        var error = AddManualSlaves(descType, slaveTypes);
                        if (error != MetaError.None) {
                            return MetaRes.Err(error);
                        }
                        continue;
                    } else {
                        return MetaRes.Err(MetaError.DescriptionIsNotObject);
                    }
                }

                var desc = descType.Obj.Peel();

                var methodBuilder = new MethodBuilder();
                methodBuilder
                .Modifiers("public static")
                .Return("void")
                .Name("FromJSON")
                .Parameter("this JSONType type")
                .Parameter($"ref {typeName} val")
                .BuildPreBody(builder, ref indent);

                if (desc.ContainsKey("__is_ref")) {
                    var ifNull = new IfBuilder();
                    ifNull
                    .Condition("val == null")
                    .BuildCondition(builder, ref indent);

                    var create = new AssignBuilder();
                    create
                    .Left("val")
                    .Right($"new {typeName}()")
                    .Build(builder, ref indent);

                    ifNull
                    .BuildEndCondition(builder, ref indent);

                    builder.Append('\n');
                }

                var ifNotObj = new IfBuilder();
                ifNotObj
                .Condition("type.Obj.IsNone()")
                .BuildCondition(builder, ref indent);

                var returnIfNoObj = new ReturnBuilder();
                returnIfNoObj
                .Body("")
                .Build(builder, ref indent);

                ifNotObj
                .BuildEndCondition(builder, ref indent);

                builder.Append('\n');

                var declareRoot = new DeclareBuilder();
                declareRoot
                .Type("Dictionary<string, JSONType>")
                .Name("root")
                .Assign("type.Obj.Peel()")
                .Build(builder, ref indent);

                builder.Append('\n');

                foreach (var fieldPair in desc) {
                    var field = fieldPair.Key;

                    if (field.StartsWith("__")) {
                        continue;
                    }

                    var typeJSON = fieldPair.Value;

                    if (typeJSON.Str.IsNone()) {
                        return MetaRes.Err(MetaError.FieldTypeIsNotString);
                    }

                    var type = typeJSON.Str.Peel();

                    if (IsPrimitiveType(type)) {
                        slaveTypes.Add(type);
                    }

                    if (type.StartsWith("enum ")) {
                        slaveTypes.Add("int");
                    }

                    var isNullable = false;
                    if (type.EndsWith("?")) {
                        isNullable = true;
                        type = type.Substring(0, type.Length - 1);
                    }

                    var ifContains = new IfBuilder();
                    ifContains
                    .Condition($"root.ContainsKey(\"{field}\")")
                    .BuildCondition(builder, ref indent);

                    if (isNullable) {
                        var ifFieldNull = new IfBuilder();
                        ifFieldNull
                        .Condition($"root[\"{field}\"].Null.IsSome()")
                        .BuildCondition(builder, ref indent);

                        var assignNull = new AssignBuilder();
                        assignNull
                        .Left($"val.{field}")
                        .Right("null")
                        .Build(builder, ref indent);

                        ifFieldNull.BuildElse(builder, ref indent);

                        var declareActual = new DeclareBuilder();
                        declareActual
                        .Type(type)
                        .Name("actual")
                        .Assign($"default({type})")
                        .Build(builder, ref indent);

                        IndentMaster.Add(indent, builder);
                        builder.Append($"root[\"{field}\"].FromJSON(ref actual);\n");

                        var assignActual = new AssignBuilder();
                        assignActual
                        .Left($"val.{field}")
                        .Right("actual")
                        .Build(builder, ref indent);

                        ifFieldNull.BuildEndCondition(builder, ref indent);
                    } else {
                        IndentMaster.Add(indent, builder);
                        builder.Append($"root[\"{field}\"].FromJSON(ref val.{field});\n");
                    }

                    ifContains
                    .BuildEndCondition(builder, ref indent);

                    builder.Append('\n');
                }

                methodBuilder.BuildPostBody(builder, ref indent);

                builder.Append('\n');
            }

            foreach (var slave in slaveTypes) {
                var type = slave;
                var isEnum = false;
                if (slave.StartsWith("enum ")) {
                    isEnum = true;
                    type = slave.Substring(5);
                }

                var methodBuilder = new MethodBuilder();

                methodBuilder
                .Modifiers("public static")
                .Return("void")
                .Name("FromJSON")
                .Parameter("this JSONType type")
                .Parameter($"ref {type} val")
                .BuildPreBody(builder, ref indent);

                if (type == "string") {
                    var ifStr = new IfBuilder();
                    ifStr
                    .Condition("type.Str.IsSome()")
                    .BuildCondition(builder, ref indent);

                    var assignVal = new AssignBuilder();
                    assignVal
                    .Left("val")
                    .Right("type.Str.Peel()")
                    .Build(builder, ref indent);

                    ifStr
                    .BuildEndCondition(builder, ref indent);
                } else if (type == "bool") {
                    var ifBool = new IfBuilder();
                    ifBool
                    .Condition("type.Bool.IsSome()")
                    .BuildCondition(builder, ref indent);

                    var assignVal = new AssignBuilder();
                    assignVal
                    .Left("val")
                    .Right("type.Bool.Peel()")
                    .Build(builder, ref indent);

                    ifBool
                    .BuildEndCondition(builder, ref indent);
                } else if (IsNumType(type)) {
                    var ifNum = new IfBuilder();
                    ifNum
                    .Condition("type.Num.IsSome()")
                    .BuildCondition(builder, ref indent);

                    var declareNumStr = new DeclareBuilder();
                    declareNumStr
                    .Type("string")
                    .Name("numStr")
                    .Assign("type.Num.Peel()")
                    .Build(builder, ref indent);

                    builder.Append('\n');

                    var declareStyle = new DeclareBuilder();
                    declareStyle
                    .Type("NumberStyles")
                    .Name("style")
                    .Assign("NumberStyles.AllowDecimalPoint")
                    .Build(builder, ref indent);

                    IndentMaster.Add(indent, builder);
                    builder.Append("style |= NumberStyles.AllowExponent;\n");
                    IndentMaster.Add(indent, builder);
                    builder.Append("style |= NumberStyles.AllowLeadingSign;\n");

                    builder.Append('\n');

                    IndentMaster.Add(indent, builder);
                    var invCulture = "CultureInfo.InvariantCulture";
                    builder.Append($"{type}.TryParse(numStr, style, {invCulture}, out val);\n");

                    ifNum
                    .BuildEndCondition(builder, ref indent);
                } else if (isEnum) {
                    var ifNum = new IfBuilder();
                    ifNum
                    .Condition("type.Num.IsSome()")
                    .BuildCondition(builder, ref indent);

                    var declareNum = new DeclareBuilder();
                    declareNum
                    .Type("int")
                    .Name("num")
                    .Assign("default(int)")
                    .Build(builder, ref indent);

                    IndentMaster.Add(indent, builder);
                    builder.Append("type.FromJSON(ref num);\n");

                    var assignVal = new AssignBuilder();
                    assignVal
                    .Left("val")
                    .Right($"({type})num")
                    .Build(builder, ref indent);

                    ifNum
                    .BuildEndCondition(builder, ref indent);
                } else if (type.EndsWith("[]") || type.StartsWith("List<")) {
                    var isArr = type.EndsWith("[]");

                    string subtype = null;
                    if (isArr) {
                        subtype = type.Substring(0, type.Length - 2);
                    } else {
                        subtype = type.Substring(5, type.Length - 6);
                    }

                    var ifArr = new IfBuilder();
                    ifArr
                    .Condition("type.Arr.IsSome()")
                    .BuildCondition(builder, ref indent);

                    var declareList = new DeclareBuilder();
                    declareList
                    .Type("List<JSONType>")
                    .Name("list")
                    .Assign("type.Arr.Peel()")
                    .Build(builder, ref indent);

                    var createVal = new AssignBuilder();
                    createVal
                    .Left("val");

                    if (isArr) {
                        createVal
                        .Right($"new {subtype}[list.Count]");
                    } else {
                        createVal
                        .Right($"new List<{subtype}>(list.Count)");
                    }

                    createVal
                    .Build(builder, ref indent);

                    builder.Append('\n');

                    var forBuilder = new ForBuilder();
                    forBuilder
                    .Init("int i = 0")
                    .Condition("i < list.Count")
                    .Increment("i++")
                    .BuildPre(builder, ref indent);

                    var declareElement = new DeclareBuilder();
                    declareElement
                    .Type(subtype)
                    .Name("element")
                    .Assign($"default({subtype})")
                    .Build(builder, ref indent);

                    IndentMaster.Add(indent, builder);
                    builder.Append("list[i].FromJSON(ref element);\n");

                    if (isArr) {
                        var assignElem = new AssignBuilder();
                        assignElem
                        .Left("val[i]")
                        .Right("element")
                        .Build(builder, ref indent);
                    } else {
                        IndentMaster.Add(indent, builder);
                        builder.Append("val.Add(element);\n");
                    }

                    forBuilder.BuildPost(builder, ref indent);

                    ifArr
                    .BuildEndCondition(builder, ref indent);
                } else if (type.StartsWith("Dictionary<")) {
                    var ifObj = new IfBuilder();
                    ifObj
                    .Condition("type.Obj.IsSome()")
                    .BuildCondition(builder, ref indent);

                    string subtype = null;
                    var err = DictValueType(type, ref subtype);
                    if (err != MetaError.None) {
                        return MetaRes.Err(err);
                    }

                    var initVal = new AssignBuilder();
                    initVal
                    .Left("val")
                    .Right($"new {type}()")
                    .Build(builder, ref indent);

                    var declareObj = new DeclareBuilder();
                    declareObj
                    .Type("Dictionary<string, JSONType>")
                    .Name("obj")
                    .Assign("type.Obj.Peel()")
                    .Build(builder, ref indent);

                    var foreachBuilder = new ForeachBuilder();
                    foreachBuilder
                    .Variable("pair")
                    .Enumerable("obj")
                    .BuildPre(builder, ref indent);

                    var declarePairVal = new DeclareBuilder();
                    declarePairVal
                    .Type(subtype)
                    .Name("pairVal")
                    .Assign($"default({subtype})")
                    .Build(builder, ref indent);

                    IndentMaster.Add(indent, builder);
                    builder.Append("pair.Value.FromJSON(ref pairVal);\n");

                    var assignPair = new AssignBuilder();
                    assignPair
                    .Left("val[pair.Key]")
                    .Right("pairVal")
                    .Build(builder, ref indent);

                    foreachBuilder
                    .BuildPost(builder, ref indent);

                    ifObj
                    .BuildEndCondition(builder, ref indent);
                }

                methodBuilder
                .BuildPostBody(builder, ref indent);

                builder.Append('\n');
            }

            layoutBuilder
            .BuildPostMethods(builder, ref indent);

            return MetaRes.Code(builder.ToString());
        }

        public static MetaRes GenerateToJSON(
            JSONType description,
            string mainNamespace,
            List<string> usings
        ) {
            if (description.Obj.IsNone()) {
                return MetaRes.Err(MetaError.RootIsNotObject);
            }

            var objs = description.Obj.Peel();

            LayoutBuilder layoutBuilder = new LayoutBuilder();
            StringBuilder builder = new StringBuilder();
            int indent = 0;

            layoutBuilder = layoutBuilder
            .Using("System.Collections.Generic")
            .Using("System.Globalization");

            for (int i = 0; i < usings.Count; i++) {
                layoutBuilder = layoutBuilder.Using(usings[i]);
            }

            layoutBuilder
            .Namespace(mainNamespace)
            .Class("public static", "ToJSONExtensions")
            .BuildPreMethods(builder, ref indent);

            var slaveTypes = new HashSet<string>();

            foreach (var typeDescPair in objs) {
                var descType = typeDescPair.Value;

                var typeName = typeDescPair.Key;
                if (descType.Obj.IsNone()) {
                    if (typeName == "__slaves") {
                        var error = AddManualSlaves(descType, slaveTypes);
                        if (error != MetaError.None) {
                            return MetaRes.Err(error);
                        }
                        continue;
                    } else {
                        return MetaRes.Err(MetaError.DescriptionIsNotObject);
                    }
                }

                var desc = descType.Obj.Peel();

                var instName = "val";
                var rootName = "root";

                var methodBuilder = new MethodBuilder();
                methodBuilder
                .Modifiers("public static")
                .Return("JSONType")
                .Name("ToJSON")
                .Parameter($"this {typeName} {instName}")
                .BuildPreBody(builder, ref indent);

                if (desc.ContainsKey("__is_ref")) {
                    var ifNullBuilder = new IfBuilder();
                    ifNullBuilder
                    .Condition($"{instName} == null")
                    .BuildCondition(builder, ref indent);

                    var returnNull = new ReturnBuilder();
                    returnNull
                    .Body("JSONType.Make()")
                    .Build(builder, ref indent);

                    ifNullBuilder.BuildEndCondition(builder, ref indent);
                    builder.Append('\n');
                }

                var declareRootBuilder = new DeclareBuilder();
                declareRootBuilder
                .Type("Dictionary<string, JSONType>")
                .Name(rootName)
                .Initialize()
                .Build(builder, ref indent);

                builder.Append('\n');

                var fieldCount = 0;
                foreach (var fieldPair in desc) {
                    fieldCount++;

                    var fieldName = fieldPair.Key;

                    if (fieldName.StartsWith("__")) {
                        continue;
                    }

                    var fieldTypeOpt = fieldPair.Value;
                    if (fieldTypeOpt.Str.IsNone()) {
                        return MetaRes.Err(MetaError.FieldTypeIsNotString);
                    }
                    var fieldType = fieldTypeOpt.Str.Peel();

                    bool isNullable = false;
                    if (fieldType.EndsWith("?")) {
                        isNullable = true;
                        fieldType = fieldType.Substring(0, fieldType.Length - 1);
                    }

                    if (IsPrimitiveType(fieldType)) {
                        slaveTypes.Add(fieldType);
                    }


                    var ifFieldNull = new IfBuilder();
                    var assignField = fieldName;
                    if (isNullable) {
                        ifFieldNull
                        .Condition($"{instName}.{fieldName} != null")
                        .BuildCondition(builder, ref indent);
                        assignField = $"{fieldName}.Value";
                    }

                    var assign = new AssignBuilder();
                    assign
                    .Left($"{rootName}[\"{fieldName}\"]")
                    .Right($"{instName}.{assignField}.ToJSON()")
                    .Build(builder, ref indent);

                    if (isNullable) {
                        ifFieldNull
                        .BuildEndCondition(builder, ref indent);
                    }
                }

                builder.Append('\n');
                IndentMaster.Add(indent, builder);
                builder.Append($"return JSONType.Make({rootName});\n");
                methodBuilder.BuildPostBody(builder, ref indent);

                builder.Append('\n');
            }

            foreach (var slave in slaveTypes) {
                var type = slave;
                if (slave.StartsWith("enum ")) {
                    type = slave.Substring(5);
                }

                var slaveMethodBuilder = new MethodBuilder();
                slaveMethodBuilder
                .Modifiers("public static")
                .Return("JSONType")
                .Name("ToJSON")
                .Parameter($"this {type} val")
                .BuildPreBody(builder, ref indent);

                var err = GenToMethodBody(slave, "val", builder, ref indent);
                if (err != MetaError.None) {
                    return MetaRes.Err(err);
                }

                slaveMethodBuilder
                .BuildPostBody(builder, ref indent);

                builder.Append('\n');
            }

            layoutBuilder.BuildPostMethods(builder, ref indent);

            return MetaRes.Code(builder.ToString());
        }

        private static MetaError GenToMethodBody(
            string type,
            string name,
            StringBuilder builder,
            ref int indent
        ) {
            if (type == "string" || type == "bool") {
                var returnBuilder = new ReturnBuilder();
                returnBuilder
                .Body($"JSONType.Make({name})")
                .Build(builder, ref indent);
            } else if (IsNumType(type)) {
                var declareTypeNum = new DeclareBuilder();
                var numName = "num";
                var numTypeName = $"{numName}Type";
                var numStrName = $"{numName}Str";

                declareTypeNum
                .Type("JSONType")
                .Name(numTypeName)
                .Initialize()
                .Build(builder, ref indent);

                var declareNum = new DeclareBuilder();
                declareNum
                .Type(type)
                .Name(numName)
                .Assign(name)
                .Build(builder, ref indent);

                var declareNumStr = new DeclareBuilder();
                declareNumStr
                .Type("string")
                .Name(numStrName)
                .Assign($"{numName}.ToString(CultureInfo.InvariantCulture)")
                .Build(builder, ref indent);

                var assignNum = new AssignBuilder();
                assignNum
                .Left($"{numTypeName}.Num")
                .Right($"Option<string>.Some({numStrName})")
                .Build(builder, ref indent);

                builder.Append('\n');

                var returnNum = new ReturnBuilder();
                returnNum
                .Body(numTypeName)
                .Build(builder, ref indent);
            } else if (type.EndsWith("[]") || type.StartsWith("List<")) {
                var isArray = type.EndsWith("[]");

                var ifNull = new IfBuilder();

                ifNull
                .Condition($"{name} == null")
                .BuildCondition(builder, ref indent);

                var returnNull = new ReturnBuilder();
                returnNull
                .Body("JSONType.Make()")
                .Build(builder, ref indent);

                ifNull
                .BuildEndCondition(builder, ref indent);

                builder.Append('\n');

                var lenName = "len";

                var assignArrLen = new AssignBuilder();
                assignArrLen
                .Left($"int {lenName}");

                if (isArray) {
                    assignArrLen
                    .Right($"{name}.Length");
                } else {
                    assignArrLen
                    .Right($"{name}.Count");
                }
                assignArrLen.Build(builder, ref indent);

                var listName = "list";

                var declareList = new DeclareBuilder();
                declareList
                .Type("List<JSONType>")
                .Name(listName)
                .Initialize(lenName)
                .Build(builder, ref indent);

                var forBuilder = new ForBuilder();
                forBuilder
                .Init("int i = 0")
                .Condition($"i < {lenName}")
                .Increment("i++")
                .BuildPre(builder, ref indent);

                string elemType;
                if (isArray) {
                    elemType = type.Substring(0, type.Length - 2);
                } else {
                    elemType = type.Substring(5, type.Length - 6);
                }

                var elemName = "elem";

                IndentMaster.Add(indent, builder);
                builder.Append($"{listName}.Add({name}[i].ToJSON());\n");

                forBuilder.BuildPost(builder, ref indent);

                builder.Append('\n');

                var returnList = new ReturnBuilder();
                returnList
                .Body($"JSONType.Make({listName})")
                .Build(builder, ref indent);
            } else if (type.StartsWith("Dictionary<")) {
                var ifNotNull = new IfBuilder();
                ifNotNull
                .Condition($"{name} == null")
                .BuildCondition(builder, ref indent);

                var returnNull = new ReturnBuilder();
                returnNull
                .Body("JSONType.Make()")
                .Build(builder, ref indent);

                ifNotNull.BuildEndCondition(builder, ref indent);

                builder.Append('\n');

                var jsonDictName = "dict";
                var declareDict = new DeclareBuilder();
                declareDict
                .Type("Dictionary<string, JSONType>")
                .Name(jsonDictName)
                .Initialize()
                .Build(builder, ref indent);

                var foreachBuilder = new ForeachBuilder();
                foreachBuilder
                .Variable("pair")
                .Enumerable(name)
                .BuildPre(builder, ref indent);

                string valType = null;
                var err = DictValueType(type, ref valType);
                if (err != MetaError.None) {
                    return err;
                }

                var assignVal = new AssignBuilder();
                assignVal
                .Left("dict[pair.Key.ToString()]")
                .Right("pair.Value.ToJSON()")
                .Build(builder, ref indent);

                foreachBuilder.BuildPost(builder, ref indent);

                builder.Append('\n');

                var returnDict = new ReturnBuilder();
                returnDict
                .Body($"JSONType.Make({jsonDictName})")
                .Build(builder, ref indent);
            } else if (type.StartsWith("enum ")) {
                var declareAsInt = new DeclareBuilder();
                declareAsInt
                .Type("int")
                .Name("asInt")
                .Assign("(int)val")
                .Build(builder, ref indent);

                var returnJson = new ReturnBuilder();
                returnJson
                .Body("asInt.ToJSON()")
                .Build(builder, ref indent);
            } else {
                return MetaError.UnsupportedPrimitive;
            }

            return MetaError.None;
        }

        private static bool IsNumType(string type) {
            bool isNum = false;
            isNum = isNum || type == "sbyte" || type == "byte";
            isNum = isNum || type == "short" || type == "ushort";
            isNum = isNum || type == "int" || type == "uint";
            isNum = isNum || type == "long" || type == "ulong";
            isNum = isNum || type == "float" || type == "double" || type == "decimal";

            return isNum;
        }

        private static bool IsPrimitiveType(string type) {
            var isPrim = false;
            isPrim = isPrim || type == "string" || type == "bool";
            isPrim = isPrim || IsNumType(type);
            isPrim = isPrim || type.EndsWith("[]");
            isPrim = isPrim || type.StartsWith("List<");
            isPrim = isPrim || type.StartsWith("Dictionary<");
            isPrim = isPrim || type.StartsWith("enum ");

            return isPrim;
        }

        private static MetaError DictValueType(string type, ref string valType) {
            var commaIndex = type.IndexOf(',');
            if (commaIndex < 0) {
                return MetaError.MissingCommaInDictionary;
            }

            var closeAngleIndex = type.LastIndexOf('>');
            if (closeAngleIndex < 0) {
                return MetaError.MissingClosingInDictionary;
            }

            var len = closeAngleIndex - commaIndex - 2;
            valType = type.Substring(commaIndex + 2, len);

            return MetaError.None;
        }

        private static MetaError AddManualSlaves(JSONType descType, HashSet<string> slaveTypes) {
            if (descType.Arr.IsNone() ) {
                return MetaError.SlaveInfoMustBeArray;
            }

            var slavesArr = descType.Arr.Peel();
            foreach (var slave in slavesArr) {
                if (slave.Str.IsNone()) {
                    return MetaError.SlaveMustBeString;
                }

                var slaveName = slave.Str.Peel();
                if (IsPrimitiveType(slaveName)) {
                    slaveTypes.Add(slaveName);
                } else {
                    return MetaError.SlaveMustBePrimitive;
                }
            }

            return MetaError.None;
        }
    }
}
