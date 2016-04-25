using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HONGLI.Entity;

namespace HONGLI.Repository
{
    public class ProductRepository
    {
        public View_ProductUserItem SaveProduct(Product_User product_user, Product_Item product_item)
        {
            var userItem = new View_ProductUserItem();

            var productId = 0;
            using (var context = new E2JOINDB())
            {
                //事务提交
                context.Product_User.Add(product_user);
                context.SaveChanges();

                productId = product_user.Id;
                product_item.UserId = productId;
                context.Product_Item.Add(product_item);
                context.SaveChanges();
            }

            userItem = GetProduct(productId);

            return userItem;
        }

        public View_ProductUserItem GetProduct(int productId)
        {
            var result = new View_ProductUserItem();

            using (var context = new E2JOINDB())
            {
                var query = context.View_ProductUserItem.Where(t => t.Id == productId).FirstOrDefault();

                if (query != null)
                    result = query;
            }

            return result;
        }
    }
}
