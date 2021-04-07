using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMDT.Models
{
    public class _TrangCaNhan
    {
        public IEnumerable<_FullAccountInfo> _FullAccountInfo { get; set; }
        public IEnumerable<_FullProduct> _FullProduct { get; set; }
    }
    public class _TrangChu
    {
        public IEnumerable<Category_Product> _Category_Product { get; set; }
        public IEnumerable<_FullProduct> _FullProduct { get; set; }
    }
}