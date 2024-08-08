//from https://iquilezles.org/articles/smoothsteps/
//https://easings.net

void PiecewiseQuad_float(in float x,out float outVal)
{
	outVal = (x<0.5) ? 2.0*x*x : 2.0*x*(2.0-x)-1.0;
}

float PiecewiseQuad(float x)
{
	return (x<0.5) ? 2.0*x*x : 2.0*x*(2.0-x)-1.0;
}

void PiecewisePoly_float(in float x,in float n,out float outVal)
{
	outVal = (x<0.5) ? 0.5*pow(2.0*x,n):1.0-0.5*pow(2.0*(1.0-x), n);
}

float PiecewisePoly(float x,float n)
{
	return (x<0.5) ? 0.5*pow(2.0*x,n):1.0-0.5*pow(2.0*(1.0-x), n);
}

void CubicPoly_float(in float x,out float outVal)
{
	outVal = x*x*(3.0-2.0*x);
}

float CubicPoly(float x)
{
	return x*x*(3.0-2.0*x);
}

void QuarticPoly_float(in float x,out float outVal)
{
	outVal = x*x*(2.0-x*x);
}

float QuarticPoly(float x)
{
	return x*x*(2.0-x*x);
}

void QuinticPoly_float(in float x,out float outVal)
{
	outVal = x*x*x*(x*(x*6.0-15.0)+10.0);
}

float QuinticPoly(float x)
{
	return x*x*x*(x*(x*6.0-15.0)+10.0);
}

void QuadRat_float(in float x,out float outVal)
{
	outVal = x*x/(2.0*x*x-2.0*x+1.0);
}

float QuadRat(float x)
{
	return x*x/(2.0*x*x-2.0*x+1.0);
}

void CubicRat_float(in float x,out float outVal)
{
	outVal = x*x*x/(3.0*x*x-3.0*x+1.0);
}

float CubicRat(float x)
{
	return x*x*x/(3.0*x*x-3.0*x+1.0);
}

void Rat_float(in float x,in float n,out float outVal)
{
	outVal = pow(x,n)/(pow(x,n)+pow(1.0-x,n));
}

float Rat(float x,float n)
{
	return pow(x,n)/(pow(x,n)+pow(1.0-x,n));
}

void Trig_float(in float x,out float outVal)
{
	outVal = 0.5-0.5*cos(PI*x);
}

float Trig(float x)
{
	return 0.5-0.5*cos(PI*x);
}

float EaseOutExpo(float x)
{	
	return (x == 1) ? 1 : 1 - pow(2, -10 * x);
}

void EaseOutExpo_float(in float x,out float y)
{
	y = (x == 1) ? 1 : 1 - pow(2, -10 * x);
}

float EaseOutBounce(float x){
	float n1 = 7.5625;
	float d1 = 2.75;
	
	if (x < 1 / d1) {
	    return n1 * x * x;
	} else if (x < 2 / d1) {
	    return n1 * (x -= 1.5 / d1) * x + 0.75;
	} else if (x < 2.5 / d1) {
	    return n1 * (x -= 2.25 / d1) * x + 0.9375;
	} else {
	    return n1 * (x -= 2.625 / d1) * x + 0.984375;
	}
}

float EaseInBounce(float x)
{
	return 1 - EaseOutBounce(1 - x);
}


float EaseOutElastic(float x)
{
	float c4 = (2 * PI) / 3;
	
	return x == 0
	  ? 0
	  : x == 1
	  ? 1
	  : pow(2, -10 * x) * sin((x * 10 - 0.75) * c4) + 1;
}

float EaseInOutBounce(float x)
{
	return x < 0.5
	  ? (1 - EaseOutBounce(1 - 2 * x)) / 2
	  : (1 + EaseOutBounce(2 * x - 1)) / 2;
}


float EaseInElastic(float x)
{
	float c4 = (2 * PI) / 3;
	
	return x == 0
	  ? 0
	  : x == 1
	  ? 1
	  : -pow(2, 10 * x - 10) * sin((x * 10 - 10.75) * c4);
}

void EaseInElastic_float(in float x, out float y)
{
	y = EaseInElastic(x);
}

void SmoothStep_float(in float x,in int i,in float n,out float outVal)
{
	switch(i)
	{
		case 0:
			outVal = x;
			return;
		case 1:
			outVal = PiecewiseQuad(x);
			return;
		case 2:
			outVal = CubicPoly(x);
			return;
		case 3:
			outVal = QuarticPoly(x);
			return;
		case 4:
			outVal =  QuinticPoly(x);
			return;
		case 5:
			outVal = QuadRat(x);
			return;
		case 6:
			outVal = CubicRat(x);
			return;
		case 7:
			outVal = Trig(x);
			return;
		case 8:
			outVal = PiecewisePoly(x,n);
			return;
		case 9:
			outVal = Rat(x,n);
			return;
		case 10:
			outVal = EaseOutExpo(x);
			return;
		case 11:
			outVal = EaseOutBounce(x);
			return;
		case 12:
			outVal = EaseInBounce(x);
			return;
		case 13:
			outVal = EaseInOutBounce(x);
			return;
		case 14:
			outVal = EaseOutElastic(x);
			return;
		case 15:
			outVal = EaseInElastic(x);
			return;
		default:
			outVal = x;
			return;
	}
}
