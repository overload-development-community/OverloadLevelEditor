#pragma once

#include <stdint.h>

bool unity_crunch_unpack_level(const uint8_t* data, uint32_t data_size, uint32_t level_index, void** ret, uint32_t* ret_size);
void *unity_crunch_unpack_init(const void* hdr_data, uint32_t hdr_size, int level_index, uint32_t* pDataOfs, uint32_t* pDataSize, uint32_t* ret_size);
bool unity_crunch_unpack_level_data(void* pContext, int level_index, const void* pData, uint32_t pDataSize, void *pDst, int pDstSize);
void unity_crunch_unpack_done(void *pContext);
