using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.ComponentModel;
using System.IO;
using TInput = System.String;
using TOutput = System.String;

namespace DeferredPipeline
{
    [ContentProcessor(DisplayName = "Deferred Renderer Model")]
    public class DeferredRendererModel : ModelProcessor
    {
        string m_textureDirectory;
        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input was null");
            }
            
            m_textureDirectory = Path.GetDirectoryName(input.Identity.SourceFilename);
            LookUpTextures(input);

            return base.Process(input, context);
        }

        [Browsable(false)]
        public override bool GenerateTangentFrames
        {
            get
            {
                return true;
            }
            set
            { }
        }

        static IList<String> AcceptableVertexChannelNames = new string[]
        {
            VertexChannelNames.TextureCoordinate(0),
            VertexChannelNames.Normal(0),
            VertexChannelNames.Binormal(0),
            VertexChannelNames.Tangent(0)
        };

        protected override void ProcessVertexChannel(GeometryContent geometry, int vertexChannelIndex, ContentProcessorContext context)
        {
            String vertexChannelName = geometry.Vertices.Channels[vertexChannelIndex].Name;

            //If this channel has an acceptable names, process it as normal.
            if (AcceptableVertexChannelNames.Contains(vertexChannelName))
            {
                base.ProcessVertexChannel(geometry, vertexChannelIndex, context);
            }
            else
            {
                geometry.Vertices.Channels.Remove(vertexChannelName);
            }
        }

        //[DisplayName("Diffuse Map Texture")]
        //[Description("If set, this file will be used as the diffuse map on the model, " +
        //"overriding anything found in the opaque data.")]
        //[DefaultValue("")]
        //public string DiffuseMapTexture
        //{
        //    get { return m_diffuseMapTexture; }
        //    set { m_diffuseMapTexture = value; }
        //}
        //private string m_diffuseMapTexture;

        //[DisplayName("Diffuse Map Key")]
        //[Description("If set, this file will be used as the diffuse map on the model, overriding anything found in the opaque data.")]
        //[DefaultValue("DiffuseMap")]
        //public string DiffuseMapKey
        //{
        //    get
        //    {
        //        return m_diffuseMapKey;
        //    }
        //    set
        //    {
        //        m_diffuseMapKey = value;
        //    }
        //}
        //private string m_diffuseMapKey = "DiffuseMap";



        [DisplayName("Normal Map Texture")]
        [Description("If set, this file will be used as the normal map on the model, " +
        "overriding anything found in the opaque data.")]
        [DefaultValue("")]
        public string NormalMapTexture
        {
            get { return m_normalMapTexture; }
            set { m_normalMapTexture = value; }
        }
        private string m_normalMapTexture;

        [DisplayName("Normal Map Key")]
        [Description("If set, this file will be used as the normal map on the model, overriding anything found in the opaque data.")]
        [DefaultValue("NormalMap")]
        public string NormalMapKey
        {
            get
            {
                return m_normalMapKey;
            }
            set
            {
                m_normalMapKey = value;
            }
        }
        private string m_normalMapKey = "NormalMap";

        private void LookUpTextures(NodeContent node)
        {
            MeshContent mesh = node as MeshContent;

            if (mesh != null)
            {
                #region DiffuseTexture


                ////THis will contain the path to the diffuse map texture
                //string diffuseMapPath;

                ////If the DiffuseMapTexture property is set, we use that diffuse map for all meshes in the model
                ////This overrides anything else
                //if (!String.IsNullOrEmpty(NormalMapTexture))
                //{
                //    diffuseMapPath = DiffuseMapTexture;
                //}
                //else
                //{
                //    //If not set, looks into opqaue data of the model
                //    diffuseMapPath = mesh.OpaqueData.GetValue<string>(DiffuseMapKey, null);
                //}

                ////If DiffuseMapTexture property was not used, and key not found in mode, than diffuseMap should be null.
                //if (diffuseMapPath == null)
                //{
                //    //Last ditch attempt for a normal map
                //    diffuseMapPath = Path.Combine(m_textureDirectory, mesh.Name + "_c.tga");

                //    if (!File.Exists(diffuseMapPath))
                //    {
                //        //normalMapPath = "Models\\null_normal.tga";
                //    }
                //}
                //else
                //{
                //    diffuseMapPath = Path.Combine(m_textureDirectory, diffuseMapPath);
                //}

                #endregion DiffuseTexture

                #region NormalTexture
                //THis will contain the path to the normal map texture
                string normalMapPath;

                //If the NormalMapTexture property is set, we use that normal map for all meshes in the model
                //This overrides anything else
                if (!String.IsNullOrEmpty(NormalMapTexture))
                {
                    normalMapPath = NormalMapTexture;
                }
                else
                {
                    //If not set, looks into opqaue data of the model
                    normalMapPath = mesh.OpaqueData.GetValue<string>(NormalMapKey, null);
                }

                //If NormalMapTexture property was not used, and key not found in mode, than normalMap should be null.
                if (normalMapPath == null)
                {
                    //Last ditch attempt for a normal map
                    normalMapPath = Path.Combine(m_textureDirectory, mesh.Name + "_n.tga");

                    if (!File.Exists(normalMapPath))
                    {
                        //normalMapPath = "Models\\null_normal.tga";
                    }
                }
                else
                {
                    normalMapPath = Path.Combine(m_textureDirectory, normalMapPath);
                }

                #endregion NormalTexture

                #region SpecularTexture

                //THis will contain the path to the normal map texture
                string specMapPath;

                //If the NormalMapTexture property is set, we use that normal map for all meshes in the model
                //This overrides anything else
                if (!String.IsNullOrEmpty(SpecularMapTexture))
                {
                    specMapPath = SpecularMapTexture;
                }
                else
                {
                    //If not set, looks into opqaue data of the model
                    specMapPath = mesh.OpaqueData.GetValue<string>(SpecularMapKey, null);
                }

                //If NormalMapTexture property was not used, and key not found in mode, than normalMap should be null.
                if (specMapPath == null)
                {
                    //Last ditch attempt for a normal map
                    specMapPath = Path.Combine(m_textureDirectory, mesh.Name + "_s.tga");

                    if (!File.Exists(specMapPath))
                    {
                        //specMapPath = "\\Models\\null_specular.tga";
                    }
                }
                else
                {
                    specMapPath = Path.Combine(m_textureDirectory, specMapPath);
                }


                #endregion SpecularTexture

                foreach (var geometry in mesh.Geometry)
                {
                    //if (geometry.Material.Textures.ContainsKey(DiffuseMapKey))
                    //{
                    //    ExternalReference<TextureContent> texRef = geometry.Material.Textures[DiffuseMapKey];
                    //    geometry.Material.Textures.Remove(DiffuseMapKey);
                    //    geometry.Material.Textures.Add("DiffuseMap", texRef);

                    //}
                    //else
                    //{
                    //    geometry.Material.Textures.Add("DiffuseMap", new ExternalReference<TextureContent>(diffuseMapPath));
                    //}

                    if (geometry.Material.Textures.ContainsKey(NormalMapKey))
                    {
                        ExternalReference<TextureContent> texRef = geometry.Material.Textures[NormalMapKey];
                        geometry.Material.Textures.Remove(NormalMapKey);
                        geometry.Material.Textures.Add("NormalMap", texRef);

                    }
                    else
                    {
                        geometry.Material.Textures.Add("NormalMap", new ExternalReference<TextureContent>(normalMapPath));
                    }

                    if (geometry.Material.Textures.ContainsKey(SpecularMapKey))
                    {
                        ExternalReference<TextureContent> texRef = geometry.Material.Textures[SpecularMapKey];
                        geometry.Material.Textures.Remove(SpecularMapKey);
                        geometry.Material.Textures.Add("SpecularMap", texRef);
                    }
                    else
                        geometry.Material.Textures.Add("SpecularMap",
                                    new ExternalReference<TextureContent>(specMapPath));
                }
            }

            //Go through children
            foreach (var child in node.Children)
            {
                LookUpTextures(child);
            }
        }

        [DisplayName("Specular Map Texture")]
        [Description("If set, this file will be used as the specular map on the model, " +
        "overriding anything found in the opaque data.")]
        [DefaultValue("")]
        public string SpecularMapTexture
        {
            get { return m_specMapTexture; }
            set { m_specMapTexture = value; }
        }
        private string m_specMapTexture;

        [DisplayName("Specular Map Key")]
        [Description("If set, this file will be used as the specular map on the model, overriding anything found in the opaque data.")]
        [DefaultValue("SpecularMap")]
        public string SpecularMapKey
        {
            get
            {
                return m_specMapKey;
            }
            set
            {
                m_specMapKey = value;
            }
        }
        private string m_specMapKey = "SpecularMap";

        protected override MaterialContent ConvertMaterial(MaterialContent material, ContentProcessorContext context)
        {
            EffectMaterialContent deferredShadingMaterial = new EffectMaterialContent();
            deferredShadingMaterial.Effect = new ExternalReference<EffectContent>("Shaders\\RenderGBuffer.fx");

            //Copy textures in the original material to the new normal mapping material if they are relevant to our renderer.
            //The LookUpTextures function has added the normal map and specular map textures to the Textures colletion so that will be copied as well
            foreach (var kvp in material.Textures)
            {
                if ((kvp.Key == "Texture") || (kvp.Key == "NormalMap") || (kvp.Key == "SpecularMap"))
                {
                    deferredShadingMaterial.Textures.Add(kvp.Key, kvp.Value);
                }
            }

            return context.Convert<MaterialContent, MaterialContent>(deferredShadingMaterial, typeof(MaterialProcessor).Name);
        }
    }
}