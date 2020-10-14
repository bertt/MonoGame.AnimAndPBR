﻿using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Graphics;

using GLTFMATERIAL = SharpGLTF.Schema2.Material;

namespace Microsoft.Xna.Framework.Content.Pipeline.Graphics
{
    /// <summary>
    /// Gltf loading factory using in built monogame's effects like <see cref="BasicEffect"/> and <see cref="SkinnedEffect"/>
    /// </summary>
    /// <remarks>
    /// Monogame's BasicEffect and SkinnedEffect use Phong's shading, while glTF uses PBR shading,
    /// so given monogame's limitations, we try to guess the most appropiate values to have a
    /// reasonably good looking renders.
    /// 
    /// Also, for SkinnedEffect, skinning is limited to 72 bones.
    /// </remarks>
    public class BasicMeshFactory : GLTFMeshFactory
    {
        #region lifecycle

        public BasicMeshFactory(GraphicsDevice device)
            : base(device) { }

        #endregion

        #region API

        protected override Type GetPreferredVertexType(IMeshPrimitiveDecoder<GLTFMATERIAL> srcPrim)
        {
            return srcPrim.JointsWeightsCount > 0 ? typeof(VertexBasicSkinned) : typeof(VertexPositionNormalTexture);
        }

        protected override Effect ConvertMaterial(MaterialContent srcMaterial, bool mustSupportSkinning)
        {
            return PBREffectsFactory.CreateClassicEffect(srcMaterial, mustSupportSkinning, Device, tobj => FileContentTextureFactory.UseTexture(tobj as Byte[]));
        }

        #endregion

        #region vertex types

        struct VertexBasicSkinned : IVertexType
        {
            #region static

            private static VertexDeclaration _VDecl = CreateVertexDeclaration();

            public static VertexDeclaration CreateVertexDeclaration()
            {
                int offset = 0;

                var a = new VertexElement(offset, VertexElementFormat.Vector3, VertexElementUsage.Position, 0);
                offset += 3 * 4;

                var b = new VertexElement(offset, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0);
                offset += 3 * 4;

                var c = new VertexElement(offset, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0);
                offset += 2 * 4;

                var d = new VertexElement(offset, VertexElementFormat.Byte4, VertexElementUsage.BlendIndices, 0);
                offset += 4 * 1;

                var e = new VertexElement(offset, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0);
                offset += 4 * 4;

                return new VertexDeclaration(a, b, c, d, e);
            }

            #endregion

            #region data

            public VertexDeclaration VertexDeclaration => _VDecl;

            public Vector3 Position;
            public Vector3 Normal;
            public Vector2 TextureCoordinate;
            public Framework.Graphics.PackedVector.Byte4 BlendIndices;
            public Vector4 BlendWeight;

            #endregion
        }

        #endregion
    }
}
