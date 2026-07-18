#if UNITY_EDITOR
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;


namespace FlexiSprite
{
    public static class AnimatedSpriteShaderGenerator
    {
        const string OutputPath = "Assets/AnimatedSprite/runtime/AnimatedSpriteShader/AnimatedSpriteShader.Generated.cs";


        public static void Generate()
        {
            var sb = new StringBuilder();
            var t = typeof(AnimatedSpriteShader);
            var all = EffectRegistry.All;

            sb.AppendLine("// AUTO-GENERATED — do not edit. Run AnimatedSprite/Generate Push Methods.");
            sb.AppendLine("namespace FlexiSprite");
            sb.AppendLine("{");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine();
            sb.AppendLine("public partial class AnimatedSpriteShader");
            sb.AppendLine("{");

            for (int ei = 0; ei < all.Length; ei++)
            {
                var def = all[ei];
                sb.AppendLine($"    void PushEffect_{def.id}(MaterialPropertyBlock b)");
                sb.AppendLine("    {");

                if (def.enableShader != null)
                    sb.AppendLine($"        b.SetFloat(\"{def.enableShader}\", {def.enableField} ? 1f : 0f);");

                if (!string.IsNullOrEmpty(def.maskField))
                {
                    var maskFi = t.GetField(def.maskField, BindingFlags.Public | BindingFlags.Instance);
                    if (maskFi != null)
                    {
                        string cast = maskFi.FieldType == typeof(int) ? "(float)" : "(float)(int)";
                        sb.AppendLine($"        b.SetFloat(\"{def.maskShader}\", {cast}{def.maskField});");
                    }
                }

                foreach (var p in def.parms)
                {
                    var fi = t.GetField(p.field, BindingFlags.Public | BindingFlags.Instance);
                    if (fi == null) continue;

                    switch (p.type)
                    {
                        case ParamType.Float:
                            sb.AppendLine($"        b.SetFloat(\"{p.shader}\", {p.field});");
                            break;
                        case ParamType.Int:
                            if (fi.FieldType == typeof(bool))
                                sb.AppendLine($"        b.SetFloat(\"{p.shader}\", {p.field} ? 1f : 0f);");
                            else
                                sb.AppendLine($"        b.SetFloat(\"{p.shader}\", (float){p.field});");
                            break;
                        case ParamType.Color:
                        case ParamType.HDRColor:
                            sb.AppendLine($"        b.SetColor(\"{p.shader}\", {p.field});");
                            break;
                        case ParamType.Vector4:
                            sb.AppendLine($"        b.SetVector(\"{p.shader}\", {p.field});");
                            break;
                        case ParamType.Vector2:
                            sb.AppendLine($"        b.SetVector(\"{p.shader}\", new Vector4({p.field}.x, {p.field}.y, 0, 0));");
                            break;
                        case ParamType.Texture:
                            sb.AppendLine($"        if ({p.field}) b.SetTexture(\"{p.shader}\", {p.field});");
                            break;
                    }
                }

                sb.AppendLine("    }");
                sb.AppendLine();
            }

            for (int ei = 0; ei < all.Length; ei++)
            {
                var def = all[ei];
                sb.AppendLine($"    void PushEffect_{def.id}(Material m)");
                sb.AppendLine("    {");

                if (def.enableShader != null)
                    sb.AppendLine($"        m.SetFloat(\"{def.enableShader}\", {def.enableField} ? 1f : 0f);");

                if (!string.IsNullOrEmpty(def.maskField))
                {
                    var maskFi = t.GetField(def.maskField, BindingFlags.Public | BindingFlags.Instance);
                    if (maskFi != null)
                    {
                        string cast = maskFi.FieldType == typeof(int) ? "(float)" : "(float)(int)";
                        sb.AppendLine($"        m.SetFloat(\"{def.maskShader}\", {cast}{def.maskField});");
                    }
                }

                foreach (var p in def.parms)
                {
                    var fi = t.GetField(p.field, BindingFlags.Public | BindingFlags.Instance);
                    if (fi == null) continue;

                    switch (p.type)
                    {
                        case ParamType.Float:
                            sb.AppendLine($"        m.SetFloat(\"{p.shader}\", {p.field});");
                            break;
                        case ParamType.Int:
                            if (fi.FieldType == typeof(bool))
                                sb.AppendLine($"        m.SetFloat(\"{p.shader}\", {p.field} ? 1f : 0f);");
                            else
                                sb.AppendLine($"        m.SetFloat(\"{p.shader}\", (float){p.field});");
                            break;
                        case ParamType.Color:
                        case ParamType.HDRColor:
                            sb.AppendLine($"        m.SetColor(\"{p.shader}\", {p.field});");
                            break;
                        case ParamType.Vector4:
                            sb.AppendLine($"        m.SetVector(\"{p.shader}\", {p.field});");
                            break;
                        case ParamType.Vector2:
                            sb.AppendLine($"        m.SetVector(\"{p.shader}\", new Vector4({p.field}.x, {p.field}.y, 0, 0));");
                            break;
                        case ParamType.Texture:
                            sb.AppendLine($"        if ({p.field}) m.SetTexture(\"{p.shader}\", {p.field});");
                            break;
                    }
                }

                sb.AppendLine("    }");
                sb.AppendLine();
            }

            sb.AppendLine("    void PushEffects(MaterialPropertyBlock b)");
            sb.AppendLine("    {");
            foreach (var def in all)
                sb.AppendLine($"        PushEffect_{def.id}(b);");
            sb.AppendLine("    }");
            sb.AppendLine();

            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// Generated material-direct push for GUI variants. Mirrors <see cref=\"PushEffects(MaterialPropertyBlock)\"/>.");
            sb.AppendLine("    /// Matches the partial declaration in AnimatedSpriteShader.cs. If the generator hasn't");
            sb.AppendLine("    /// been run since the asset was imported, this body is absent and the call is elided,");
            sb.AppendLine("    /// in which case PushGUI() additionally calls PushEffectsToMat() (reflection-based) so");
            sb.AppendLine("    /// effects still push. Running Full Rebuild in Master Setup generates this file.");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine("    partial void PushEffectsToMaterial(Material mat)");
            sb.AppendLine("    {");
            // Sentinel write first — PushGUI checks this to know the generated body ran.
            sb.AppendLine("        mat.SetFloat(\"_AIO_GenPushSentinel\", 1f);");
            foreach (var def in all)
                sb.AppendLine($"        PushEffect_{def.id}(mat);");
            sb.AppendLine("    }");
            sb.AppendLine();

            sb.AppendLine("    partial void PushEffectToMaterial(Material mat, EffectDef def)");
            sb.AppendLine("    {");
            sb.AppendLine("        mat.SetFloat(\"_AIO_GenPushSentinel\", 1f);");
            sb.AppendLine("        switch (def.id)");
            sb.AppendLine("        {");
            foreach (var def in all)
                sb.AppendLine($"            case EffectId.{def.id}: PushEffect_{def.id}(mat); return;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine();

            sb.AppendLine("    void PushEffect(MaterialPropertyBlock b, EffectDef def)");
            sb.AppendLine("    {");
            sb.AppendLine("        switch (def.id)");
            sb.AppendLine("        {");
            foreach (var def in all)
                sb.AppendLine($"            case EffectId.{def.id}: PushEffect_{def.id}(b); return;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine();

            sb.AppendLine("    void MarkEnabledEffectsDirty()");
            sb.AppendLine("    {");
            for (int i = 0; i < all.Length; i++)
                sb.AppendLine($"        if ({all[i].enableField}) SetEffectDirtyBit({i});");
            sb.AppendLine("    }");
            sb.AppendLine();

            sb.AppendLine("    // ---- Snapshot fields ----");
            foreach (var def in all)
            {
                // Enable bool
                sb.AppendLine($"    bool _prev_{def.enableField};");

                // Mask field
                if (!string.IsNullOrEmpty(def.maskField))
                {
                    var maskFi = t.GetField(def.maskField, BindingFlags.Public | BindingFlags.Instance);
                    if (maskFi != null)
                        sb.AppendLine($"    {CSharpTypeName(maskFi.FieldType)} _prev_{def.maskField};");
                }

                // Params
                foreach (var p in def.parms)
                {
                    var fi = t.GetField(p.field, BindingFlags.Public | BindingFlags.Instance);
                    if (fi == null) continue;
                    sb.AppendLine($"    {CSharpTypeName(fi.FieldType)} _prev_{p.field};");
                }
            }
            sb.AppendLine();

            for (int ei = 0; ei < all.Length; ei++)
            {
                var def = all[ei];
                sb.AppendLine($"    bool CheckEffect_{def.id}()");
                sb.AppendLine("    {");
                sb.AppendLine("        bool changed = false;");

                if (!string.IsNullOrEmpty(def.maskField))
                {
                    var maskFi = t.GetField(def.maskField, BindingFlags.Public | BindingFlags.Instance);
                    if (maskFi != null)
                        sb.AppendLine($"        if ({def.maskField} != _prev_{def.maskField}) {{ _prev_{def.maskField} = {def.maskField}; changed = true; }}");
                }

                foreach (var p in def.parms)
                {
                    var fi = t.GetField(p.field, BindingFlags.Public | BindingFlags.Instance);
                    if (fi == null) continue;
                    sb.AppendLine($"        if ({p.field} != _prev_{p.field}) {{ _prev_{p.field} = {p.field}; changed = true; }}");
                }

                sb.AppendLine("        return changed;");
                sb.AppendLine("    }");
                sb.AppendLine();
            }

            sb.AppendLine("    void CheckEnabledEffectsForChanges()");
            sb.AppendLine("    {");

            // Enable toggle pass — any toggle requires a full Push (keywords must rebuild)
            for (int i = 0; i < all.Length; i++)
            {
                sb.AppendLine($"        if ({all[i].enableField} != _prev_{all[i].enableField}) {{ _prev_{all[i].enableField} = {all[i].enableField}; _dirty = true; }}");
            }
            sb.AppendLine("        if (_dirty) return;");
            sb.AppendLine();

            // Param check pass — only for enabled effects
            for (int i = 0; i < all.Length; i++)
                sb.AppendLine($"        if ({all[i].enableField} && CheckEffect_{all[i].id}()) SetEffectDirtyBit({i});");

            sb.AppendLine("    }");
            sb.AppendLine();

            sb.AppendLine("    void InitEffectSnapshot()");
            sb.AppendLine("    {");
            foreach (var def in all)
            {
                sb.AppendLine($"        _prev_{def.enableField} = {def.enableField};");

                if (!string.IsNullOrEmpty(def.maskField))
                {
                    var maskFi = t.GetField(def.maskField, BindingFlags.Public | BindingFlags.Instance);
                    if (maskFi != null)
                        sb.AppendLine($"        _prev_{def.maskField} = {def.maskField};");
                }

                foreach (var p in def.parms)
                {
                    var fi = t.GetField(p.field, BindingFlags.Public | BindingFlags.Instance);
                    if (fi == null) continue;
                    sb.AppendLine($"        _prev_{p.field} = {p.field};");
                }
            }
            sb.AppendLine("    }");

            sb.AppendLine("}");
            sb.AppendLine("}");

            var dir = Path.GetDirectoryName(OutputPath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(OutputPath, sb.ToString());
            AssetDatabase.Refresh();
            Debug.Log($"[AnimSprite] Generated {all.Length} push/check methods → {OutputPath}");
        }

        static string CSharpTypeName(System.Type t)
        {
            if (t == typeof(float)) return "float";
            if (t == typeof(int)) return "int";
            if (t == typeof(bool)) return "bool";
            if (t == typeof(Color)) return "Color";
            if (t == typeof(Vector2)) return "Vector2";
            if (t == typeof(Vector3)) return "Vector3";
            if (t == typeof(Vector4)) return "Vector4";
            if (t == typeof(Texture2D)) return "Texture2D";
            return t.Name;
        }
    }
}
#endif