float2 clrand2(uint id, __global uint4* clrng)
{
	uint s1 = clrng[id].x;
	uint s2 = clrng[id].y;
	uint s3 = clrng[id].z;
	uint b;

	b = (((s1 << 13) ^ s1) >> 19);
	s1 = (((s1 & 4294967294) << 12) ^ b);
	b = (((s2 << 2) ^ s2) >> 25);
	s2 = (((s2 & 4294967288) << 4) ^ b);
	b = (((s3 << 3) ^ s3) >> 11);
	s3 = (((s3 & 4294967280) << 17) ^ b);

	float2 result;
	result.x = (float)((s1 ^ s2 ^ s3) * 2.3283064365e-10);

	b = (((s1 << 13) ^ s1) >> 19);
	s1 = (((s1 & 4294967294) << 12) ^ b);
	b = (((s2 << 2) ^ s2) >> 25);
	s2 = (((s2 & 4294967288) << 4) ^ b);
	b = (((s3 << 3) ^ s3) >> 11);
	s3 = (((s3 & 4294967280) << 17) ^ b);

	result.y = (float)((s1 ^ s2 ^ s3) * 2.3283064365e-10);

	clrng[id] = (uint4)(s1, s2, s3, b);

	return result;
}

__kernel void Mandelbrot(
	const uint  width,
	const uint  height,
	const float reMin,
	const float reMax,
	const float imMin,
	const float imMax,
	const uint maxIter,
	__global uint4* clrng,
	__global uchar4* clout)
{
	const float escapeOrbit = 4.0f;

	float2 rand = clrand2(get_global_id(0), clrng);
	float2 c = (float2)(mix(reMin, reMax, rand.x), mix(imMin, imMax, rand.y));

	float2 z = 0.0f;
	int iter = 0;

	if (!(((c.x - 0.25f)*(c.x - 0.25f) + (c.y * c.y))*(((c.x - 0.25f)*(c.x - 0.25f) + (c.y * c.y)) + (c.x - 0.25f)) < 0.25f * c.y * c.y))  //main cardioid
	{
		if (!((c.x + 1.0f) * (c.x + 1.0f) + (c.y * c.y) < 0.0625f))            //2nd order period bulb
		{
			if (!((((c.x + 1.309f)*(c.x + 1.309f)) + c.y*c.y) < 0.00345f))    //smaller bulb left of the period-2 bulb
			{
				if (!((((c.x + 0.125f)*(c.x + 0.125f)) + (c.y - 0.744f)*(c.y - 0.744f)) < 0.0088f))      // smaller bulb bottom of the main cardioid
				{
					if (!((((c.x + 0.125f)*(c.x + 0.125f)) + (c.y + 0.744f)*(c.y + 0.744f)) < 0.0088f))  //smaller bulb top of the main cardioid
					{
						while ((iter < maxIter) && ((z.x * z.x + z.y * z.y) < escapeOrbit))
						{
							z = (float2)(z.x * z.x - z.y * z.y, (z.x * z.y * 2.0f)) + c;
							iter++;
						}
					}
				}
			}
		}
	}

	int x = (c.x - reMin) / (reMax - reMin) * width;
	int y = height - (c.y - imMin) / (imMax - imMin) * height;

	if ((x >= 0) && (y >= 0) && (x < width) && (y < height))
	{		
		int i = x + y * width;
		float clr = 0.0f;
		if (iter < maxIter)// && iter > 0)
		{
			// b&w
			//clr = 255.0f;

			// filled
			//clr = length(z) / escapeOrbit * 255.0f;

			// smoothed
			float k = 1.0f / half_log(escapeOrbit);
			clr = 5.0f + iter - half_log(0.5f) * k - half_log(half_log(sqrt(z.x * z.x + z.y * z.y))) * k;		
		}

		clr = clamp(clr, 0.0f, 255.0f);
		clr = (clr + clout[i].x) / 2.0f;

		clout[i].x = (uchar)clr;
		clout[i].y = (uchar)clr;
		clout[i].z = (uchar)clr;
		clout[i].w = 255;
	} // if iter x y width height
} // __kernel