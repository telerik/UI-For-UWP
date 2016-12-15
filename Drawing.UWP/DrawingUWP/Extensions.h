#pragma once

#include "Enumerations.h"

using namespace Windows::UI;
using namespace Windows::Foundation;
using namespace Windows::UI::Text;
using namespace Telerik::UI::Drawing;

const float DefaultDPI = 96.0f;

class Extensions
{

public:
	static D2D1::ColorF ToColor(Color color)
	{
		float r = color.R / 255.0f;
		float g = color.G / 255.0f;
		float b = color.B / 255.0f;
		float a = color.A / 255.0f;
		return D2D1::ColorF(r, g, b, a);
	}

	static D2D1_POINT_2F ToPoint(DoublePoint pt)
	{
		D2D1_POINT_2F d2dPoint =
		{
			d2dPoint.x = static_cast<float>(pt.X),
			d2dPoint.y = static_cast<float>(pt.Y)
		};

		return d2dPoint;
	}

	static D2D1_POINT_2F ToPoint(Point pt)
	{
		return D2D1::Point2F(pt.X, pt.Y);
	}

	static D2D1_RECT_F ToRect(Rect rect)
	{
		return D2D1::RectF(rect.X, rect.Y, rect.Right, rect.Bottom);
	}

	static RECT ToRectL(Rect rect)
	{
		RECT rectL;
		rectL.left = static_cast<LONG>(rect.Left);
		rectL.top = static_cast<LONG>(rect.Top);
		rectL.right = static_cast<LONG>(rect.Left + rect.Width);
		rectL.bottom = static_cast<LONG>(rect.Left + rect.Height);

		return rectL;
	}

	static Point ConvertPointToPixels(Point pt, float dpi)
	{
		Point point;
		point.X = RoundFloat(pt.X * dpi / DefaultDPI);
		point.Y = RoundFloat(pt.Y * dpi / DefaultDPI);
		return point;
	}

	static DoublePoint ConvertPointToPixels(DoublePoint pt, float dpi)
	{
		DoublePoint dblPt;
		dblPt.X = RoundDouble(pt.X * dpi / DefaultDPI);
		dblPt.Y = RoundDouble(pt.Y * dpi / DefaultDPI);
		return dblPt;
	}

	static double RoundDouble(double num)
	{
		if (num >= 0)
		{
			return (double)(int)(num + 0.5f);
		}

		auto round = (double)(int)num;
		double fraction = fmod(num, 1);
		if (fraction < -0.5)
		{
			round--;
		}

		return round;
	}

	static float RoundFloat(float num)
	{
		if (num >= 0)
		{
			return (float)(int)(num + 0.5f);
		}

		auto round = (float)(int)num;
		auto fraction = fmod(num, 1);
		if (fraction < -0.5)
		{
			round--;
		}

		return round;
	}

	static Color ToXAMLColor(D2D1::ColorF color)
	{
		Color xamlColor;
		xamlColor.A = static_cast<unsigned char>(color.a * 255.0f);
		xamlColor.B = static_cast<unsigned char>(color.b * 255.0f);
		xamlColor.G = static_cast<unsigned char>(color.g * 255.0f);
		xamlColor.R = static_cast<unsigned char>(color.r * 255.0f);

		return xamlColor;
	}

	static DWRITE_FONT_WEIGHT ToDWriteFontWeight(Telerik::UI::Drawing::FontWeightName name)
	{
		return (DWRITE_FONT_WEIGHT)FontWeightFromName(name).Weight;
	}

	static FontWeight FontWeightFromName(Telerik::UI::Drawing::FontWeightName name)
	{
		switch (name)
		{
			case Telerik::UI::Drawing::FontWeightName::Black:
				return FontWeights::Black;
			case Telerik::UI::Drawing::FontWeightName::Bold:
				return FontWeights::Bold;
			case Telerik::UI::Drawing::FontWeightName::ExtraBlack:
				return FontWeights::ExtraBlack;
			case Telerik::UI::Drawing::FontWeightName::ExtraBold:
				return FontWeights::ExtraBold;
			case Telerik::UI::Drawing::FontWeightName::ExtraLight:
				return FontWeights::ExtraLight;
			case Telerik::UI::Drawing::FontWeightName::Light:
				return FontWeights::Light;
			case Telerik::UI::Drawing::FontWeightName::Medium:
				return FontWeights::Medium;
			case Telerik::UI::Drawing::FontWeightName::Normal:
				return FontWeights::Normal;
			case Telerik::UI::Drawing::FontWeightName::SemiBold:
				return FontWeights::SemiBold;
			case Telerik::UI::Drawing::FontWeightName::SemiLight:
				return FontWeights::SemiLight;
			case Telerik::UI::Drawing::FontWeightName::Thin:
				return FontWeights::Thin;
			default:
				return FontWeights::Normal;
		}
	}
};