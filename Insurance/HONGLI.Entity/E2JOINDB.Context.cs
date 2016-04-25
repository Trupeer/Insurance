﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace HONGLI.Entity
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class E2JOINDB : DbContext
    {
        public E2JOINDB()
            : base("name=E2JOINDB")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BaseInfo_UserInfo> BaseInfo_UserInfo { get; set; }
        public virtual DbSet<Order_Base> Order_Base { get; set; }
        public virtual DbSet<Order_Deliver> Order_Deliver { get; set; }
        public virtual DbSet<Order_Item> Order_Item { get; set; }
        public virtual DbSet<Order_Pay> Order_Pay { get; set; }
        public virtual DbSet<Product_Item> Product_Item { get; set; }
        public virtual DbSet<Product_User> Product_User { get; set; }
        public virtual DbSet<ReadPack> ReadPack { get; set; }
        public virtual DbSet<SYS_LogInfo> SYS_LogInfo { get; set; }
        public virtual DbSet<User_Deliver> User_Deliver { get; set; }
        public virtual DbSet<User_Invoice> User_Invoice { get; set; }
        public virtual DbSet<User_PolicyHolder> User_PolicyHolder { get; set; }
        public virtual DbSet<View_ProductUserItem> View_ProductUserItem { get; set; }
        public virtual DbSet<View_ProductV2UserItem> View_ProductV2UserItem { get; set; }
        public virtual DbSet<ProductV2_Renewal> ProductV2_Renewal { get; set; }
        public virtual DbSet<View_ProductV2UserItemPolicy> View_ProductV2UserItemPolicy { get; set; }
        public virtual DbSet<ProductV2_Item> ProductV2_Item { get; set; }
        public virtual DbSet<ProductV2_User> ProductV2_User { get; set; }
        public virtual DbSet<Order_PolicyHolder> Order_PolicyHolder { get; set; }
    
        public virtual int ReadPack_GetList(string openId, Nullable<System.DateTime> startTime, Nullable<System.DateTime> endTime, Nullable<int> status, Nullable<int> pageSize, Nullable<int> pageIndex, ObjectParameter totalPage, ObjectParameter totalRecord, ObjectParameter error)
        {
            var openIdParameter = openId != null ?
                new ObjectParameter("OpenId", openId) :
                new ObjectParameter("OpenId", typeof(string));
    
            var startTimeParameter = startTime.HasValue ?
                new ObjectParameter("StartTime", startTime) :
                new ObjectParameter("StartTime", typeof(System.DateTime));
    
            var endTimeParameter = endTime.HasValue ?
                new ObjectParameter("EndTime", endTime) :
                new ObjectParameter("EndTime", typeof(System.DateTime));
    
            var statusParameter = status.HasValue ?
                new ObjectParameter("Status", status) :
                new ObjectParameter("Status", typeof(int));
    
            var pageSizeParameter = pageSize.HasValue ?
                new ObjectParameter("PageSize", pageSize) :
                new ObjectParameter("PageSize", typeof(int));
    
            var pageIndexParameter = pageIndex.HasValue ?
                new ObjectParameter("PageIndex", pageIndex) :
                new ObjectParameter("PageIndex", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ReadPack_GetList", openIdParameter, startTimeParameter, endTimeParameter, statusParameter, pageSizeParameter, pageIndexParameter, totalPage, totalRecord, error);
        }
    }
}