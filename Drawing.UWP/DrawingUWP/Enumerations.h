#pragma once

namespace Telerik
{
	namespace UI
	{
		namespace Drawing
		{
			public enum class GeometryFillMode
			{ 
				Alternate,
				Winding
			};

			public enum class ShapeUIState
			{ 
				Normal,
				PointerOver,
				Selected
			};

			public enum class ShapeLabelVisibility
			{ 
				Auto,
				Visible,
				Hidden
			};

			public enum class ShapeRenderPrecision
			{
				Default,
				Single,
				Double
			};

			public enum class FontWeightName
			{
				/// <summary>
				/// Specifies a "Black" font weight.
				/// </summary>
				Black,

				/// <summary>
				/// Specifies a "Bold" font weight.
				/// </summary>
				Bold,

				/// <summary>
				///  Specifies an "ExtraBlack" font weight.
				/// </summary>
				ExtraBlack,

				/// <summary>
				///  Specifies an "ExtraBold" font weight.
				/// </summary>
				ExtraBold,

				/// <summary>
				///  Specifies an "ExtraLight" font weight.
				/// </summary>
				ExtraLight,

				/// <summary>
				///  Specifies a "Light" font weight.
				/// </summary>
				Light,

				/// <summary>
				///  Specifies a "Medium" font weight.
				/// </summary>
				Medium,

				/// <summary>
				///  Specifies a "Normal" font weight.
				/// </summary>
				Normal,

				/// <summary>
				///  Specifies a "SemiBold" font weight.
				/// </summary>
				SemiBold,

				/// <summary>
				///  Specifies a "SemiLight" font weight.
				/// </summary>
				SemiLight,

				/// <summary>
				///  Specifies a "Thin" font weight.
				/// </summary>
				Thin
			};

			public value class DoublePoint
			{
			public:
				double X;
				double Y;
			};

			public value class ShapeLayerParameters
			{
			public:
				int Id;
				int ZIndex;
				ShapeRenderPrecision RenderPrecision;
			};
		}
	}
}
