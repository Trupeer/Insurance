using HONGLI.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HONGLI.Repository
{
    /// <summary>
    /// 产品相关
    /// by  Lee 20160323
    /// </summary>
    public class ProductV2Repository
    {
        public View_ProductV2UserItem SaveProduct(ProductV2_User product_user, ProductV2_Item product_item)
        {
            var userItem = new View_ProductV2UserItem();

            var productId = 0;
            using (var context = new E2JOINDB())
            {
                //事务提交
                context.ProductV2_User.Add(product_user);
                context.SaveChanges();

                productId = product_user.Id;
                product_item.UserId = productId;
                context.ProductV2_Item.Add(product_item);
                context.SaveChanges();
            }

            userItem = GetProductV2(productId);

            return userItem;
        }

        public View_ProductV2UserItem GetProductV2(int productId)
        {
            var result = new View_ProductV2UserItem();

            using (var context = new E2JOINDB())
            {
                var query = context.View_ProductV2UserItem.Where(t => t.Id == productId).FirstOrDefault();

                if (query != null)
                    result = query;
            }

            return result;
        }
    }
}
