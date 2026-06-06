// <copyright file=BaseSearchDto.cs company= AMF>
// Copyright (c) AMF. All rights reserved.
// </copyright>

namespace MS_Application.DataTransferObjects.Base
{
    using MS_Application.Constants;

    public class BaseSearchDto<T>
        where T : class
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = GlobalConstants.DefaultPageSize;

        public bool Asc { get; set; } = false;

        public T? SearchParams { get; set; }

        public int Start
        {
            get
            {
                return this.Page == 0 ? 0 : (this.Page - 1) * this.PageSize;
            }
        }
    }
}
