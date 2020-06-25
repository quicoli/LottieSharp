using LottieSharp.Model.Content;
using LottieSharp.Utils;
using LottieSharp.Value;
using System.Collections.Generic;

namespace LottieSharp.Animation.Keyframe
{
    internal class ShapeKeyframeAnimation : BaseKeyframeAnimation<ShapeData, Path>
    {
        private readonly ShapeData _tempShapeData = new ShapeData();
        private readonly Path _tempPath = new Path();

        internal ShapeKeyframeAnimation(List<Keyframe<ShapeData>> keyframes) : base(keyframes)
        {
        }

        public override Path GetValue(Keyframe<ShapeData> keyframe, float keyframeProgress)
        {
            var startShapeData = keyframe.StartValue;
            var endShapeData = keyframe.EndValue;

            _tempShapeData.InterpolateBetween(startShapeData, endShapeData, keyframeProgress);
            MiscUtils.GetPathFromData(_tempShapeData, _tempPath);
            return _tempPath;
        }
    }
}