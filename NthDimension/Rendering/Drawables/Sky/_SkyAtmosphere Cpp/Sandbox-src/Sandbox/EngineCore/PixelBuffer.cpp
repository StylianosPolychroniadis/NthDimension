/*
Copyright (c) 2000, Sean O'Neil (s_p_oneil@hotmail.com)
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

* Redistributions of source code must retain the above copyright notice,
  this list of conditions and the following disclaimer.
* Redistributions in binary form must reproduce the above copyright notice,
  this list of conditions and the following disclaimer in the documentation
  and/or other materials provided with the distribution.
* Neither the name of the project nor the names of its contributors may be
  used to endorse or promote products derived from this software without
  specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

#include "EngineCore.h"


bool CPixelBuffer::LoadPNG(const char *pszFile)
{
	// Currently only supports 8-bit RGB and RGBA
	FILE *pFile = fopen(pszFile, "rb");
	if(pFile == NULL)
	{
		LogError("Unable to open %s.", pszFile);
		return false;
	}
	unsigned char header[8];
	fread(header, 1, 8, pFile);
	if(png_sig_cmp(header, 0, 8))
	{
		LogError("%s is not a valid PNG file.", pszFile);
		fclose(pFile);
		return false;
	}

	png_structp png_ptr;
	png_infop info_ptr;
	png_bytep *row_pointers;

	if((png_ptr = png_create_read_struct(PNG_LIBPNG_VER_STRING, NULL, NULL, NULL)) == NULL)
	{
		LogError("png_create_read_struct failed.");
		fclose(pFile);
		return false;
	}
	if((info_ptr = png_create_info_struct(png_ptr)) == NULL)
	{
		LogError("png_create_info_struct failed.");
		png_destroy_read_struct(&png_ptr, NULL, NULL);
		fclose(pFile);
		return false;
	}
	setjmp(png_jmpbuf(png_ptr));
	png_init_io(png_ptr, pFile);
	png_set_sig_bytes(png_ptr, 8);
	png_read_info(png_ptr, info_ptr);

	if(info_ptr->color_type == PNG_COLOR_TYPE_RGB && info_ptr->bit_depth == 8)
		Init(info_ptr->width, info_ptr->height, 1, 3, GL_RGB);
	else if(info_ptr->color_type == PNG_COLOR_TYPE_RGB_ALPHA && info_ptr->bit_depth == 8)
		Init(info_ptr->width, info_ptr->height, 1, 4, GL_RGBA);
	else
	{
		LogError("Attempting to read an unsupported format from %s.", pszFile);
		png_destroy_read_struct(&png_ptr, NULL, NULL);
		fclose(pFile);
		return false;
	}

	int nRowSize = m_nWidth * m_nChannels;
	png_read_update_info(png_ptr, info_ptr);
	setjmp(png_jmpbuf(png_ptr));
	row_pointers = new png_bytep[m_nHeight];
	for(int y=0; y<m_nHeight; y++)
		row_pointers[y] = (unsigned char *)m_pBuffer + y * nRowSize;
	png_read_image(png_ptr, row_pointers);
	delete row_pointers;

	png_destroy_read_struct(&png_ptr, NULL, NULL);
	fclose(pFile);
	return true;
}

bool CPixelBuffer::SavePNG(const char *pszFile)
{
	// Currently only supports 8-bit RGB and RGBA
	if(m_nDataType != GL_UNSIGNED_BYTE && (m_nChannels != 3 || m_nChannels != 4))
	{
		LogError("Attempting to write an unsupported format to %s.", pszFile);
		return false;
	}

	FILE *pFile = fopen(pszFile, "wb");
	if(pFile == NULL)
	{
		LogError("Unable to create %s.", pszFile);
		return false;
	}

	png_structp png_ptr;
	png_infop info_ptr;
	png_bytep *row_pointers;

	if((png_ptr = png_create_write_struct(PNG_LIBPNG_VER_STRING, NULL, NULL, NULL)) == NULL)
	{
		LogError("png_create_write_struct failed.");
		return false;
	}
	if((info_ptr = png_create_info_struct(png_ptr)) == NULL)
	{
		LogError("png_create_info_struct failed.");
		return false;
	}
	setjmp(png_jmpbuf(png_ptr));
	png_init_io(png_ptr, pFile);
	png_set_IHDR(png_ptr, info_ptr, m_nWidth, m_nHeight, 8, m_nChannels == 4 ? PNG_COLOR_TYPE_RGB_ALPHA : PNG_COLOR_TYPE_RGB, PNG_INTERLACE_NONE, PNG_COMPRESSION_TYPE_BASE, PNG_FILTER_TYPE_BASE);
	png_write_info(png_ptr, info_ptr);

	int nRowSize = m_nChannels * m_nWidth;
	row_pointers = new png_bytep[m_nHeight];
	for(int y=0; y<m_nHeight; y++)
		row_pointers[y] = (unsigned char *)m_pBuffer + y * nRowSize;
	png_write_image(png_ptr, row_pointers);
	delete row_pointers;

	png_write_end(png_ptr, NULL);
	fclose(pFile);
	return true;
}

void CPixelBuffer::MakeGlow1D()
{
	int nIndex=0;
	for(int x=0; x<m_nWidth; x++)
	{
		float fIntensity = powf((float)x / m_nWidth, 0.75f);
		for(int i=0; i<m_nChannels-1; i++)
			((unsigned char *)m_pBuffer)[nIndex++] = (unsigned char)255;
		((unsigned char *)m_pBuffer)[nIndex++] = (unsigned char)(fIntensity*255 + 0.5f);
	}
}

void CPixelBuffer::MakeGlow2(float fExpose, float fSizeDisc)
{
	float f2[2];
	CFractal fractal(2, 1, 0.1f, 2.0f);
	int n = 0;
	for(int y=0; y<m_nHeight; y++)
	{
		float fDy = (y+0.5f)/m_nHeight - 0.5f;
		for(int x=0; x<m_nWidth; x++)
		{
			float fDx = (x+0.5f)/m_nWidth - 0.5f;
			float fDist = sqrtf(fDx*fDx + fDy*fDy);
			//float fIntensity = expf(-CMath::Max(fDist-fSizeDisc,0.0f)*fExpose);
			float fIntensity = 2 - CMath::Min(2.0f, powf(2.0f, CMath::Max(fDist-fSizeDisc,0.0f)*fExpose));

			f2[0] = fDx*64.0f;
			f2[1] = fDy*64.0f;
			float fNoise = 1.0f;//fractal.fBm(f2, 4.0f) * 0.25f + 0.75f;
			((unsigned char *)m_pBuffer)[n++] = (unsigned char)(fNoise*fIntensity*255 + 0.5f);
			((unsigned char *)m_pBuffer)[n++] = (unsigned char)(fNoise*fIntensity*255 + 0.5f);
			//((unsigned char *)m_pBuffer)[n++] = (unsigned char)(fNoise*255 + 0.5f);
		}
	}
}

void CPixelBuffer::Make3DNoise(int nSeed)
{
	CFractal noise(3, nSeed, 0.5f, 2.0f);
	int n = 0;
	float fValues[3];
	for(int z=0; z<m_nDepth; z++)
	{
		fValues[2] = (float)z * 0.0625f;
		for(int y=0; y<m_nHeight; y++)
		{
			fValues[1] = (float)y * 0.0625f;
			for(int x=0; x<m_nWidth; x++)
			{
				fValues[0] = (float)x * 0.0625f;
				float fIntensity = (noise.fBm(fValues, 4.0f) + 1.0f) * 0.5f;
				if(fIntensity < 0.5f)
					fIntensity = 0.0f;
				//fIntensity = 1.0f - powf(0.9f, fIntensity*255);
				unsigned char nIntensity = (unsigned char)(fIntensity*255 + 0.5f);
				((unsigned char *)m_pBuffer)[n++] = 255;
				((unsigned char *)m_pBuffer)[n++] = nIntensity;
			}
		}
	}
}

void CPixelBuffer::MakeOpticalDepthBuffer(float fInnerRadius, float fOuterRadius, float fRayleighScaleHeight, float fMieScaleHeight)
{
	const int nSize = 128;
	const int nSamples = 50;
	const float fScale = 1.0f / (fOuterRadius - fInnerRadius);

	Init(nSize, nSize, 1, 4, GL_RGBA, GL_FLOAT);
	int nIndex = 0;
	for(int nAngle=0; nAngle<nSize; nAngle++)
	{
		// As the y tex coord goes from 0 to 1, the angle goes from 0 to 180 degrees
		float fCos = 1.0f - (nAngle+nAngle) / (float)nSize;
		float fAngle = acosf(fCos);
		CVector vRay(sinf(fAngle), cosf(fAngle), 0);	// Ray pointing to the viewpoint
		for(int nHeight=0; nHeight<nSize; nHeight++)
		{
			// As the x tex coord goes from 0 to 1, the height goes from the bottom of the atmosphere to the top
			float fHeight = DELTA + fInnerRadius + ((fOuterRadius - fInnerRadius) * nHeight) / nSize;
			CVector vPos(0, fHeight, 0);				// The position of the camera

			// If the ray from vPos heading in the vRay direction intersects the inner radius (i.e. the planet), then this spot is not visible from the viewpoint
			float B = 2.0f * (vPos | vRay);
			float Bsq = B * B;
			float Cpart = (vPos | vPos);
			float C = Cpart - fInnerRadius*fInnerRadius;
			float fDet = Bsq - 4.0f * C;
			bool bVisible = (fDet < 0 || (0.5f * (-B - sqrtf(fDet)) <= 0) && (0.5f * (-B + sqrtf(fDet)) <= 0));
			float fRayleighDensityRatio;
			float fMieDensityRatio;
			if(bVisible)
			{
				fRayleighDensityRatio = expf(-(fHeight - fInnerRadius) * fScale / fRayleighScaleHeight);
				fMieDensityRatio = expf(-(fHeight - fInnerRadius) * fScale / fMieScaleHeight);
			}
			else
			{
				// Smooth the transition from light to shadow (it is a soft shadow after all)
				fRayleighDensityRatio = ((float *)m_pBuffer)[nIndex - nSize*m_nChannels] * 0.75f;
				fMieDensityRatio = ((float *)m_pBuffer)[nIndex+2 - nSize*m_nChannels] * 0.75f;
			}

			// Determine where the ray intersects the outer radius (the top of the atmosphere)
			// This is the end of our ray for determining the optical depth (vPos is the start)
			C = Cpart - fOuterRadius*fOuterRadius;
			fDet = Bsq - 4.0f * C;
			float fFar = 0.5f * (-B + sqrtf(fDet));

			// Next determine the length of each sample, scale the sample ray, and make sure position checks are at the center of a sample ray
			float fSampleLength = fFar / nSamples;
			float fScaledLength = fSampleLength * fScale;
			CVector vSampleRay = vRay * fSampleLength;
			vPos += vSampleRay * 0.5f;

			// Iterate through the samples to sum up the optical depth for the distance the ray travels through the atmosphere
			float fRayleighDepth = 0;
			float fMieDepth = 0;
			for(int i=0; i<nSamples; i++)
			{
				float fHeight = vPos.Magnitude();
				float fAltitude = (fHeight - fInnerRadius) * fScale;
				fAltitude = CMath::Max(fAltitude, 0.0f);
				fRayleighDepth += expf(-fAltitude / fRayleighScaleHeight);
				fMieDepth += expf(-fAltitude / fMieScaleHeight);
				vPos += vSampleRay;
			}

			// Multiply the sums by the length the ray traveled
			fRayleighDepth *= fScaledLength;
			fMieDepth *= fScaledLength;

			// Store the results for Rayleigh to the light source, Rayleigh to the camera, Mie to the light source, and Mie to the camera
			((float *)m_pBuffer)[nIndex++] = fRayleighDensityRatio;
			((float *)m_pBuffer)[nIndex++] = fRayleighDepth;
			((float *)m_pBuffer)[nIndex++] = fMieDensityRatio;
			((float *)m_pBuffer)[nIndex++] = fMieDepth;
		}
	}
}

void CPixelBuffer::MakePhaseBuffer(float ESun, float Kr, float Km, float g)
{
	Km *= ESun;
	Kr *= ESun;
	float g2 = g*g;
	float fMiePart = 1.5f * (1.0f - g2) / (2.0f + g2);

	int nIndex = 0;
	for(int nAngle=0; nAngle<m_nWidth; nAngle++)
	{
		float fCos = 1.0f - (nAngle+nAngle) / (float)m_nWidth;
		float fCos2 = fCos*fCos;
		float fRayleighPhase = 0.75f * (1.0f + fCos2);
		float fMiePhase = fMiePart * (1.0f + fCos2) / powf(1.0f + g2 - 2.0f*g*fCos, 1.5f);
		((float *)m_pBuffer)[nIndex++] = fRayleighPhase * Kr;
		((float *)m_pBuffer)[nIndex++] = fMiePhase * Km;
	}
}






#ifndef FREE
#define FREE(p) { if(p) { free(p); p=NULL; } }
#endif

#define DDS_FOURCC  0x00000004
#define DDS_RGB     0x00000040
#define DDS_RGBA    0x00000041
#define DDS_VOLUME  0x00200000
#define DDS_CUBEMAP 0x00000200

#define DDS_MAGIC ('D'|('D'<<8)|('S'<<16)|(' '<<24))
#define DDS_DXT1 ('D'|('X'<<8)|('T'<<16)|('1'<<24))
#define DDS_DXT3 ('D'|('X'<<8)|('T'<<16)|('3'<<24))
#define DDS_DXT5 ('D'|('X'<<8)|('T'<<16)|('5'<<24))

#ifndef GL_COMPRESSED_RGBA_S3TC_DXT1_EXT
#define GL_COMPRESSED_RGBA_S3TC_DXT1_EXT 0x83F1
#endif

#ifndef GL_COMPRESSED_RGBA_S3TC_DXT3_EXT
#define GL_COMPRESSED_RGBA_S3TC_DXT3_EXT 0x83F2
#endif

#ifndef GL_COMPRESSED_RGBA_S3TC_DXT5_EXT
#define GL_COMPRESSED_RGBA_S3TC_DXT5_EXT 0x83F3
#endif

typedef struct
{
	unsigned short col0, col1;
	unsigned char row[4];
} DXTColorBlock_t;

typedef struct
{
	unsigned short row[4];
} DXT3AlphaBlock_t;

typedef struct
{
	unsigned char alpha0, alpha1;
	unsigned char row[6];
} DXT5AlphaBlock_t;

typedef struct
{
	unsigned long Size;
	unsigned long Flags;
	unsigned long Height;
	unsigned long Width;
	unsigned long PitchLinearSize;
	unsigned long Depth;
	unsigned long MipMapCount;
	unsigned long Reserved1[11];
	unsigned long pfSize;
	unsigned long pfFlags;
	unsigned long pfFourCC;
	unsigned long pfRGBBitCount;
	unsigned long pfRMask;
	unsigned long pfGMask;
	unsigned long pfBMask;
	unsigned long pfAMask;
	unsigned long Caps1;
	unsigned long Caps2;
	unsigned long Reserved2[3];
} DDS_Header_t;

/*
void Swap(void *byte1, void *byte2, int size)
{
	unsigned char *tmp=(unsigned char *)malloc(sizeof(unsigned char)*size);

	memcpy(tmp, byte1, size);
	memcpy(byte1, byte2, size);
	memcpy(byte2, tmp, size);

	FREE(tmp);
}

void flipDXT1Blocks(DXTColorBlock_t *Block, int NumBlocks)
{
	int i;
	DXTColorBlock_t *ColorBlock=Block;

	for(i=0;i<NumBlocks;i++)
	{
		Swap(&ColorBlock->row[0], &ColorBlock->row[3], sizeof(unsigned char));
		Swap(&ColorBlock->row[1], &ColorBlock->row[2], sizeof(unsigned char));
		ColorBlock++;
	}
}

void flipDXT3Blocks(DXTColorBlock_t *Block, int NumBlocks)
{
	int i;
	DXTColorBlock_t *ColorBlock=Block;
	DXT3AlphaBlock_t *AlphaBlock;

	for(i=0;i<NumBlocks;i++)
	{
		AlphaBlock=(DXT3AlphaBlock_t *)ColorBlock;

		Swap(&AlphaBlock->row[0], &AlphaBlock->row[3], sizeof(unsigned short));
		Swap(&AlphaBlock->row[1], &AlphaBlock->row[2], sizeof(unsigned short));
		ColorBlock++;

		Swap(&ColorBlock->row[0], &ColorBlock->row[3], sizeof(unsigned char));
		Swap(&ColorBlock->row[1], &ColorBlock->row[2], sizeof(unsigned char));
		ColorBlock++;
	}
}

void flipDXT5Alpha(DXT5AlphaBlock_t *Block)
{
	unsigned long *Bits, Bits0=0, Bits1=0;

	memcpy(&Bits0, &Block->row[0], sizeof(unsigned char)*3);
	memcpy(&Bits1, &Block->row[3], sizeof(unsigned char)*3);

	Bits=((unsigned long *)&(Block->row[0]));
	*Bits&=0xff000000;
	*Bits|=(unsigned char)(Bits1>>12)&0x00000007;
	*Bits|=(unsigned char)((Bits1>>15)&0x00000007)<<3;
	*Bits|=(unsigned char)((Bits1>>18)&0x00000007)<<6;
	*Bits|=(unsigned char)((Bits1>>21)&0x00000007)<<9;
	*Bits|=(unsigned char)(Bits1&0x00000007)<<12;
	*Bits|=(unsigned char)((Bits1>>3)&0x00000007)<<15;
	*Bits|=(unsigned char)((Bits1>>6)&0x00000007)<<18;
	*Bits|=(unsigned char)((Bits1>>9)&0x00000007)<<21;

	Bits=((unsigned long *)&(Block->row[3]));
	*Bits&=0xff000000;
	*Bits|=(unsigned char)(Bits0>>12)&0x00000007;
	*Bits|=(unsigned char)((Bits0>>15)&0x00000007)<<3;
	*Bits|=(unsigned char)((Bits0>>18)&0x00000007)<<6;
	*Bits|=(unsigned char)((Bits0>>21)&0x00000007)<<9;
	*Bits|=(unsigned char)(Bits0&0x00000007)<<12;
	*Bits|=(unsigned char)((Bits0>>3)&0x00000007)<<15;
	*Bits|=(unsigned char)((Bits0>>6)&0x00000007)<<18;
	*Bits|=(unsigned char)((Bits0>>9)&0x00000007)<<21;
}

void flipDXT5Blocks(DXTColorBlock_t *Block, int NumBlocks)
{
	DXTColorBlock_t *ColorBlock=Block;
	DXT5AlphaBlock_t *AlphaBlock;
	int i;

	for(i=0;i<NumBlocks;i++)
	{
		AlphaBlock=(DXT5AlphaBlock_t *)ColorBlock;

		flipDXT5Alpha(AlphaBlock);
		ColorBlock++;

		Swap(&ColorBlock->row[0], &ColorBlock->row[3], sizeof(unsigned char));
		Swap(&ColorBlock->row[1], &ColorBlock->row[2], sizeof(unsigned char));
		ColorBlock++;
	}
}

void flip(unsigned char *image, int width, int height, int size, int format)
{
	int linesize, i, j;

	if((format==32)||(format==24))
	{
		unsigned char *top, *bottom;

		linesize=size/height;

		top=image;
		bottom=top+(size-linesize);

		for(i=0;i<(height>>1);i++)
		{
			Swap(bottom, top, linesize);

			top+=linesize;
			bottom-=linesize;
		}
	}
	else
	{
		DXTColorBlock_t *top;
		DXTColorBlock_t *bottom;
		int xblocks=width/4;
		int yblocks=height/4;

		switch(format)
		{
			case GL_COMPRESSED_RGBA_S3TC_DXT1_EXT: 
				linesize=xblocks*8;

				for(j=0;j<(yblocks>>1);j++)
				{
					top=(DXTColorBlock_t *)(image+j*linesize);
					bottom=(DXTColorBlock_t *)(image+(((yblocks-j)-1)*linesize));

					flipDXT1Blocks(top, xblocks);
					flipDXT1Blocks(bottom, xblocks);
					Swap(bottom, top, linesize);
				}
				break;

			case GL_COMPRESSED_RGBA_S3TC_DXT3_EXT:
				linesize=xblocks*16;

				for(j=0;j<(yblocks>>1);j++)
				{
					top=(DXTColorBlock_t *)(image+j*linesize);
					bottom=(DXTColorBlock_t *)(image+(((yblocks-j)-1)*linesize));

					flipDXT3Blocks(top, xblocks);
					flipDXT3Blocks(bottom, xblocks);
					Swap(bottom, top, linesize);
				}
				break;

			case GL_COMPRESSED_RGBA_S3TC_DXT5_EXT:
				linesize=xblocks*16;

				for(j=0;j<(yblocks>>1);j++)
				{
					top=(DXTColorBlock_t *)(image+j*linesize);
					bottom=(DXTColorBlock_t *)(image+(((yblocks-j)-1)*linesize));

					flipDXT5Blocks(top, xblocks);
					flipDXT5Blocks(bottom, xblocks);
					Swap(bottom, top, linesize);
				}
				break;

			default:
				return;
		}
	}
}
*/

bool CPixelBuffer::LoadDDS(const char *pszFile)
{
	FILE *stream = fopen(pszFile, "rb");
	if(stream == NULL)
	{
		LogError("Unable to open %s.", pszFile);
		return false;
	}

	DDS_Header_t dds;
	unsigned long magic;
	fread(&magic, sizeof(unsigned long), 1, stream);

	if(magic != DDS_MAGIC)
	{
		LogError("%s is not a valid DDS file.", pszFile);
		fclose(stream);
		return 0;
	}

	fread(&dds, sizeof(DDS_Header_t), 1, stream);

	// Only support the simple RGB and RGBA formats
	if(dds.pfFlags == DDS_RGBA && dds.pfRGBBitCount == 32)
		Init(dds.Width, dds.Height, 1, 4, GL_RGBA);
	else if (dds.pfFlags == DDS_RGB  && dds.pfRGBBitCount == 32)
		Init(dds.Width, dds.Height, 1, 4, GL_RGBA);
	else if (dds.pfFlags == DDS_RGB  && dds.pfRGBBitCount == 24)
		Init(dds.Width, dds.Height, 1, 3, GL_RGB);
	else 
	{
		LogError("Attempting to read an unsupported format from %s.", pszFile);
		fclose(stream);
		return false;
	}

	fread(m_pBuffer, 1, GetBufferSize(), stream);
	fclose(stream);
	return true;
}
