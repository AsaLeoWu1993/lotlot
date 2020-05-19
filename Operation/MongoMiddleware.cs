using Entity;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Operation
{
    public class MongoMiddleware<T> : ConnectionThrottlingPipeline where T : BaseModel
    {
        public readonly FilterDefinitionBuilder<T> Builder = Builders<T>.Filter;
        public IMongoCollection<T> Collection { get; private set; } = null;
        public MongoMiddleware()
        {
            Collection = BaseMongoHelper.GetCollection<T>();
        }

        #region 查询
        /// <summary>
        /// 查询一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        public virtual T GetModel(FilterDefinition<T> filter)
        {
            return base.AddRequest(() =>
            {
                return Collection.Find(filter).FirstOrDefault();
            });
        }

        /// <summary>
        /// 查询一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        public virtual T GetModel(Expression<Func<T, bool>> filter)
        {
            return base.AddRequest(() =>
            {
                try
                {
                    return Collection.Find(filter).FirstOrDefault();
                }
                catch (Exception e)
                {
                    throw e;
                }
            });
        }

        /// <summary>
        /// 查询一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        public virtual async Task<T> GetModelAsync(FilterDefinition<T> filter)
        {
            return await base.AddRequest(Task.Run(async () =>
            {
                var task = await Collection.FindAsync(filter);
                T model = await task.FirstOrDefaultAsync();
                return model;
            }));
        }

        /// <summary>
        /// 查询一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        public virtual async Task<T> GetModelAsync(Expression<Func<T, bool>> filter)
        {
            return await base.AddRequest(Task.Run(async () =>
            {
                var task = await Collection.FindAsync(filter);
                T model = await task.FirstOrDefaultAsync();
                return model;
            }));
        }

        /// <summary>
        /// 查询一条数据
        /// </summary>
        /// <param name="filter">查询条件</param>
        /// <param name="sort">排序字段</param>
        /// <param name="desc">true:正序 false:反序</param>
        /// <returns></returns>
        public virtual T GetModel(FilterDefinition<T> filter, Expression<Func<T, object>> sort, bool desc)
        {
            return base.AddRequest(() =>
            {
                if (desc)
                    return Collection.Find(filter).SortBy(sort).FirstOrDefault();
                else
                    return Collection.Find(filter).SortByDescending(sort).FirstOrDefault();
            });
        }

        /// <summary>
        /// 查询一条数据
        /// </summary>
        /// <param name="filter">查询条件</param>
        /// <param name="sort">排序字段</param>
        /// <param name="desc">true:正序 false:反序</param>
        /// <returns></returns>
        public virtual T GetModel(Expression<Func<T, bool>> filter, Expression<Func<T, object>> sort, bool desc)
        {
            return base.AddRequest(() =>
            {
                if (desc)
                    return Collection.Find(filter).SortBy(sort).FirstOrDefault();
                else
                    return Collection.Find(filter).SortByDescending(sort).FirstOrDefault();
            });
        }

        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        public virtual List<T> GetModelList(FilterDefinition<T> filter)
        {
            return base.AddRequest(() =>
            {
                return Collection.Find(filter).ToList();
            });
        }

        public virtual List<T> GetModelList(FilterDefinition<T> filter, Expression<Func<T, object>> sort, bool desc)
        {
            return base.AddRequest(() =>
            {
                if (desc)
                {
                    var list = Collection.Find(filter).SortBy(sort).ToList();
                    return list;
                }
                else
                {
                    var list = Collection.Find(filter).SortByDescending(sort).ToList();
                    return list;
                }
            });
        }

        /// <summary>
        /// 查询所有数据列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual List<T> GetAllModel()
        {
            return base.AddRequest(() =>
            {
                var list = Collection.Find(t => true).ToList();
                return list;
            });
        }

        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        public virtual List<T> GetModelList(Expression<Func<T, bool>> filter, Expression<Func<T, object>> sort, bool desc)
        {
            return base.AddRequest(() =>
            {
                if (desc)
                {
                    var list = Collection.Find(filter).SortBy(sort).ToList();
                    return list;
                }
                else
                {
                    var list = Collection.Find(filter).SortByDescending(sort).ToList();
                    return list;
                }
            });
        }

        public virtual List<T> GetModelList(Expression<Func<T, bool>> filter)
        {
            return base.AddRequest(() =>
            {
                var list = Collection.Find(filter).ToList();
                return list;
            });
        }

        public virtual async Task<List<T>> GetModelListAsync(Expression<Func<T, bool>> filter)
        {
            return await base.AddRequest(Task.Run(async () =>
            {
                var list = await Collection.FindAsync(filter);
                return list.ToList();
            }));
        }

        public virtual async Task<List<T>> GetModelListAsync(FilterDefinition<T> filter)
        {
            return await base.AddRequest(Task.Run(async () =>
            {
                var list = await Collection.FindAsync(filter);
                return list.ToList();
            }));
        }

        /// <summary>
        /// 查询数据列表分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">查询条件</param>
        /// <param name="sort">字段排序</param>
        /// <param name="desc">true:正序  false:反序</param>
        /// <param name="start">开始页码</param>
        /// <param name="pageSize">页面数量</param>
        /// <returns></returns>
        public virtual List<T> GetModelListByPaging(FilterDefinition<T> filter, Expression<Func<T, object>> sort, bool desc, int start = 1, int pageSize = 10)
        {
            if (desc)
                return GetModelListByPaging(filter, sort, start, pageSize);
            else
                return GetModelListByPagingDesc(filter, sort, start, pageSize);
        }

        /// <summary>
        /// 查询数据列表分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">查询条件</param>
        /// <param name="sort">字段排序</param>
        /// <param name="desc">true:正序  false:反序</param>
        /// <param name="start">开始页码</param>
        /// <param name="pageSize">页面数量</param>
        /// <returns></returns>
        public virtual List<T> GetModelListByPaging(Expression<Func<T, bool>> filter, Expression<Func<T, object>> sort, bool desc, int start = 1, int pageSize = 10)
        {
            if (desc)
                return GetModelListByPaging(filter, sort, start, pageSize);
            else
                return GetModelListByPagingDesc(filter, sort, start, pageSize);
        }

        /// <summary>
        /// 查询数据列表分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">条件</param>
        /// <param name="sort">正序排序</param>
        /// <returns></returns>
        private List<T> GetModelListByPaging(FilterDefinition<T> filter, Expression<Func<T, object>> sort, int start = 1, int pageSize = 10)
        {
            return base.AddRequest(() =>
            {
                if (start == 0) start = 1;
                var list = Collection.Find(filter).SortBy(sort).Skip((start - 1) * pageSize).Limit(pageSize).ToList();
                return list;
            });
        }

        /// <summary>
        /// 查询数据列表分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">条件</param>
        /// <param name="sort">反序排序</param>
        /// <returns></returns>
        private List<T> GetModelListByPagingDesc(Expression<Func<T, bool>> filter, Expression<Func<T, object>> sort, int start = 1, int pageSize = 10)
        {
            return base.AddRequest(() =>
            {
                if (start == 0) start = 1;
                var list = Collection.Find(filter).SortByDescending(sort).Skip((start - 1) * pageSize).Limit(pageSize).ToList();
                return list;
            });
        }

        /// <summary>
        /// 查询数据列表分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">条件</param>
        /// <param name="sort">正序排序</param>
        /// <returns></returns>
        private List<T> GetModelListByPaging(Expression<Func<T, bool>> filter, Expression<Func<T, object>> sort, int start = 1, int pageSize = 10)
        {
            return base.AddRequest(() =>
            {
                if (start == 0) start = 1;
                var list = Collection.Find(filter).SortBy(sort).Skip((start - 1) * pageSize).Limit(pageSize).ToList();
                return list;
            });
        }

        /// <summary>
        /// 查询数据列表分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">条件</param>
        /// <param name="sort">反序排序</param>
        /// <returns></returns>
        private List<T> GetModelListByPagingDesc(FilterDefinition<T> filter, Expression<Func<T, object>> sort, int start = 1, int pageSize = 10)
        {
            return base.AddRequest(() =>
            {
                if (start == 0) start = 1;
                var list = Collection.Find(filter).SortByDescending(sort).Skip((start - 1) * pageSize).Limit(pageSize).ToList();
                return list;
            });
        }

        /// <summary>
        /// 获取条数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual long GetCount(FilterDefinition<T> filter)
        {
            return base.AddRequest(() =>
            {
                return Collection.CountDocuments(filter);
            });
        }

        /// <summary>
        /// 获取条数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual long GetCount(Expression<Func<T, bool>> filter)
        {
            return base.AddRequest(() =>
            {
                return Collection.CountDocuments(filter);
            });
        }

        /// <summary>
        /// 获取条数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual async Task<long> GetCountAsync(FilterDefinition<T> filter)
        {
            return await base.AddRequest(Task.Run(async () =>
            {
                return await Collection.CountDocumentsAsync(filter);
            }));
        }

        /// <summary>
        /// 获取条数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual async Task<long> GetCountAsync(Expression<Func<T, bool>> filter)
        {
            return await base.AddRequest(Task.Run(async () =>
            {
                return await Collection.CountDocumentsAsync(filter);
            }));
        }
        #endregion

        #region 修改
        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">条件</param>
        /// <param name="update">更改内容</param>
        /// <returns></returns>
        public virtual bool UpdateModelOne(FilterDefinition<T> filter, UpdateDefinition<T> update)
        {
            return base.AddRequest(() =>
            {
                var result = Collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            });
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="updateModel"></param>
        /// <returns></returns>
        public virtual bool UpdateModel(T updateModel)
        {
            return base.AddRequest(() =>
            {
                var type = updateModel.GetType();
                var id = type.GetProperty("_id").GetValue(updateModel);
                FilterDefinition<T> filter = Builder.Eq("_id", id);
                UpdateDefinition<T> update = Builders<T>.Update
                .Set("LastUpdateTime", DateTime.Now);
                foreach (var item in type.GetProperties())
                {
                    if (item.Name == "_id" || item.Name == "CreatedTime") continue;
                    var value = item.GetValue(updateModel);
                    update = update.Set(item.Name, value);
                }
                update = update.Set("LastUpdateTime", DateTime.Now);
                var result = Collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            });
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="updateModel"></param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateModelAsync(T updateModel)
        {
            return await base.AddRequest(Task.Run(async () =>
            {
                var type = updateModel.GetType();
                var id = type.GetProperty("_id").GetValue(updateModel);
                FilterDefinition<T> filter = Builder.Eq("_id", id);
                UpdateDefinition<T> update = Builders<T>.Update
                .Set("LastUpdateTime", DateTime.Now);
                foreach (var item in type.GetProperties())
                {
                    if (item.Name == "_id" || item.Name == "CreatedTime") continue;
                    var value = item.GetValue(updateModel);
                    update = update.Set(item.Name, value);
                }
                update = update.Set("LastUpdateTime", DateTime.Now);
                var result = await Collection.UpdateOneAsync(filter, update);
                return result.IsAcknowledged;
            }));
        }

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">条件</param>
        /// <param name="update">更改内容</param>
        /// <returns></returns>
        public virtual bool UpdateModelOne(Expression<Func<T, bool>> filter, UpdateDefinition<T> update)
        {
            return base.AddRequest(() =>
            {
                var result = Collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            });
        }

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">条件</param>
        /// <param name="update">更改内容</param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateModelOneAsync(Expression<Func<T, bool>> filter, UpdateDefinition<T> update)
        {
            return await base.AddRequest(Task.Run(async () =>
            {
                var result = await Collection.UpdateOneAsync(filter, update);
                return result.IsAcknowledged;
            }));
        }

        /// <summary>
        /// 修改多条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">条件</param>
        /// <param name="update">更改内容</param>
        /// <returns></returns>
        public virtual bool UpdateModelMany(FilterDefinition<T> filter, UpdateDefinition<T> update)
        {
            return base.AddRequest(() =>
            {
                var result = Collection.UpdateMany(filter, update);
                return result.IsAcknowledged;
            });
        }

        public virtual async Task<bool> UpdateModelManyAsync(FilterDefinition<T> filter, UpdateDefinition<T> update)
        {
            return await base.AddRequest(Task.Run(async () =>
            {
                var result = await Collection.UpdateManyAsync(filter, update);
                return result.IsAcknowledged;
            }));
        }

        /// <summary>
        /// 修改多条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">条件</param>
        /// <param name="update">更改内容</param>
        /// <returns></returns>
        public virtual bool UpdateModelMany(Expression<Func<T, bool>> filter, UpdateDefinition<T> update)
        {
            return base.AddRequest(() =>
            {
                var result = Collection.UpdateMany(filter, update);
                return result.IsAcknowledged;
            });
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        public virtual bool DeleteModelOne(FilterDefinition<T> filter)
        {
            return base.AddRequest(() =>
            {
                var result = Collection.DeleteOne(filter);
                return result.IsAcknowledged;
            });
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        public virtual bool DeleteModelOne(Expression<Func<T, bool>> filter)
        {
            return base.AddRequest(() =>
            {
                var result = Collection.DeleteOne(filter);
                return result.IsAcknowledged;
            });
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteModelOneAsync(Expression<Func<T, bool>> filter)
        {
            return await base.AddRequest(Task.Run(async () =>
            {
                var result = await Collection.DeleteOneAsync(filter);
                return result.IsAcknowledged;
            }));
        }

        /// <summary>
        /// 删除多条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        public virtual bool DeleteModelMany(FilterDefinition<T> filter)
        {
            return base.AddRequest(() =>
            {
                var result = Collection.DeleteMany(filter);
                return result.IsAcknowledged;
            });
        }

        /// <summary>
        /// 删除多条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        public virtual bool DeleteModelMany(Expression<Func<T, bool>> filter)
        {
            return base.AddRequest(() =>
            {
                var result = Collection.DeleteMany(filter);
                return result.IsAcknowledged;
            });
        }

        /// <summary>
        /// 删除多条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteModelManyAsync(Expression<Func<T, bool>> filter)
        {
            return await base.AddRequest(Task.Run(async () =>
            {
                var result = await Collection.DeleteManyAsync(filter);
                return result.IsAcknowledged;
            }));
        }

        /// <summary>
        /// 删除多条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteModelManyAsync(FilterDefinition<T> filter)
        {
            return await base.AddRequest(Task.Run(async () =>
            {
                var result = await Collection.DeleteManyAsync(filter);
                return result.IsAcknowledged;
            }));
        }
        #endregion

        #region 添加
        /// <summary>
        /// 添加实体到数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        public virtual void InsertModel(T model)
        {
            base.AddRequest(() =>
            {
                Collection.InsertOne(model);
            });
        }

        /// <summary>
        /// 添加实体到数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        public virtual async Task InsertModelAsync(T model)
        {
            await base.AddRequest(Task.Run(async () =>
            {
                await Collection.InsertOneAsync(model);
            }));
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public virtual async Task InsertManyAsync(List<T> list)
        {
            await base.AddRequest(Task.Run(async () =>
            {
                await Collection.InsertManyAsync(list);
            }));
        }
        #endregion
    }
}
