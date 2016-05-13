using HONGLI.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HONGLI.Repository
{
    public class ProductV3Repository
    {
        public View_ProductV2UserItem SaveProduct(ProductV2_Item product_item)
        {
            var userItem = new View_ProductV2UserItem();

            var productId = 0;
            var productiemId = 0;
            using (var context = new E2JOINDB())
            {
                //事务提交
                productId = Convert.ToInt32(product_item.UserId);
                context.Entry(product_item).State = EntityState.Modified;
                context.SaveChanges();
                productiemId = product_item.Id;
            }
            userItem = GetProductV2(productId);

            return userItem;
        }

        public View_ProductV2UserItem GetProductV2(int productId)
        {
            var result = new View_ProductV2UserItem();

            using (var context = new E2JOINDB())
            {
                var query = context.View_ProductV2UserItem.Where(t => t.ItemId == productId).FirstOrDefault();

                if (query != null)
                    result = query;
            }

            return result;
        }
        public int SaveProductUser(ProductV2_User product_user)
        {
            int userid;
            using (var context = new E2JOINDB())
            {
                //事务提交
                context.ProductV2_User.Add(product_user);
                context.SaveChanges();
                userid = product_user.Id;
            }

            return userid;
        }
        public int Editproductitem(ProductV2_Item product_item)
        {
            int id;
            using (var context = new E2JOINDB())
            {
                //事务提交
                context.ProductV2_Item.Attach(product_item);
                context.Entry(product_item).Property(t => t.Id).IsModified = true;
                context.Entry(product_item).Property(t => t.SubmitStatus).IsModified = true;
                context.Entry(product_item).Property(t => t.SubmitResult).IsModified = true;
                context.Entry(product_item).Property(t => t.BizNo).IsModified = true;
                context.Entry(product_item).Property(t => t.ForceNo).IsModified = true;
                context.Entry(product_item).Property(t => t.ForceRate_Channel).IsModified = true;
                context.Entry(product_item).Property(t => t.TaxRate_Channel).IsModified = true;
                context.Entry(product_item).Property(t => t.BizRate_Channel).IsModified = true;
                context.Entry(product_item).Property(t => t.BizRate).IsModified = true;
                context.Entry(product_item).Property(t => t.BizAfterCoupon).IsModified = true;
                context.Entry(product_item).Property(t => t.ForceRate).IsModified = true;
                context.Entry(product_item).Property(t => t.ForceAfterCoupon).IsModified = true;
                context.Entry(product_item).Property(t => t.TaxRate).IsModified = true;
                context.Entry(product_item).Property(t => t.TaxAfterCoupon).IsModified = true;
                context.Entry(product_item).Property(t => t.TotalAfterCoupon).IsModified = true;
                context.Entry(product_item).Property(t => t.PrepaidAmount).IsModified = true;
                context.SaveChanges();
                id = product_item.Id;
            }

            return id;
        }
        public int SaveProductRenewal(ProductV2_Renewal product_renewal)
        {
            int id;
            using (var context = new E2JOINDB())
            {
                //事务提交
                context.ProductV2_Renewal.Add(product_renewal);
                context.SaveChanges();
                id = product_renewal.Id;
            }

            return id;
        }
        /// <summary>
        /// 单独保存报价信息
        /// </summary>
        /// <param name="product_renewal"></param>
        /// <returns></returns>
        public int SaveProductItem(ProductV2_Item productv2_item)
        {
            int id;
            using (var context = new E2JOINDB())
            {
                //事务提交
                context.ProductV2_Item.Add(productv2_item);
                context.SaveChanges();
                id = productv2_item.Id;
            }

            return id;
        }

        /// <summary>
        /// 修改用户选择状态（选择前清除掉所有选择状态）
        /// </summary>
        /// <param name="productItemId"></param>
        /// <returns></returns>
        public int EditProductItemUserCheck(int productItemId, int UserId)
        {
            int result = -1;
            ProductV2_Item productv2_item = new ProductV2_Item();
            productv2_item.Id = productItemId;
            productv2_item.UserSelection = true;
            using (var context = new E2JOINDB())
            {
                context.Database.ExecuteSqlCommand("UPDATE dbo.ProductV2_Item SET UserSelection=0 WHERE UserId=" + UserId);
                context.SaveChanges();
                context.ProductV2_Item.Attach(productv2_item);
                context.Entry(productv2_item).Property(t => t.Id).IsModified = true;
                context.Entry(productv2_item).Property(t => t.UserSelection).IsModified = true;
                context.SaveChanges();
                result = productv2_item.Id;
            }

            return result;
        }
        public Order_Base GetOrderByOrderCode(string ordercode)
        {
            Order_Base model = new Order_Base();
            using (var context = new E2JOINDB())
            {
                model = context.Order_Base.Where(a => a.OrderCode == ordercode).FirstOrDefault();
                return model;
            }
        }
        public int CheckOrderBase(int UserId)
        {
            int result = -1;
            string sql = string.Format("SELECT OrderCode FROM dbo.Order_Base WHERE ProductItemId IN (SELECT Id FROM dbo.ProductV2_Item WHERE UserId={0})", UserId);
            using (var db = new E2JOINDB())
            {
                var query = db.Database.SqlQuery<CheckOrderCodeList>(sql.ToString()).ToList();
                if (query.Count > 0)
                {
                    foreach(var item in query)
                    {
                        db.Database.ExecuteSqlCommand(string.Format("DELETE FROM dbo.Order_PolicyHolder WHERE OrderCode='{0}'", item.OrderCode));
                        db.Database.ExecuteSqlCommand(string.Format("DELETE FROM dbo.Order_Pay WHERE OrderCode='{0}'", item.OrderCode));
                        db.Database.ExecuteSqlCommand(string.Format("DELETE FROM dbo.Order_Item WHERE OrderCode='{0}'", item.OrderCode));
                        db.Database.ExecuteSqlCommand(string.Format("DELETE FROM dbo.Order_Deliver WHERE OrderCode='{0}'", item.OrderCode));
                        db.Database.ExecuteSqlCommand(string.Format("DELETE FROM dbo.Order_Base WHERE OrderCode='{0}'",item.OrderCode));
                        db.SaveChanges();
                        result = 1;
                    }
                }
                return result;
            }
        }
        public class CheckOrderCodeList
        {
            public string  OrderCode { get; set; }
        }
        public Order_Item GetOrdertemByOrderCode(string ordercode)
        {
            Order_Item model = new Order_Item();
            using (var context = new E2JOINDB())
            {
                model = context.Order_Item.Where(a => a.OrderCode == ordercode).FirstOrDefault();
                return model;
            }
        }
    }
}
