using System;
using NHibernate.Cfg;
using HiloCore;
using System.Linq;
using System.Collections.Generic;
using NHibernate;

namespace HiloWithNH {

    class Program {

        private static ISessionFactory sessionFactory;

        static void Main(string[] args) {
            var cfg = new Configuration();
            cfg.Configure("hibernate.config");
            sessionFactory = cfg.BuildSessionFactory();

            var entities = CreateSampleEntities(50);
            SaveEntities(entities);
            QueryEntities();

            Console.ReadLine();
            sessionFactory.Dispose();
        }

        private static void QueryEntities() {
            using (var session = sessionFactory.OpenSession()) {
                var query = from entity in session.Query<SampleEntity>()
                    group entity by entity.Text into g
                    select new {
                        Key = g.Key,
                        Count = g.Count(),
                        MinId = g.Min(e => e.Id),
                        MaxId = g.Max(e => e.Id)
                    };
                var result = query.ToList();
                Console.WriteLine(result.Count);
            }
        }

        private static void SaveEntities(IList<SampleEntity> entities) {
            using (var session = sessionFactory.OpenSession()) {
                // session.SetBatchSize(entities.Count);
                var tx = session.BeginTransaction();
                try {
                    tx.Begin();
                    foreach (var entity in entities) {
                        session.Save(entity);
                    }
                    session.Flush();
                    tx.Commit();
                }
                catch (Exception ex) {
                    tx.Rollback();
                    Console.WriteLine(ex);
                }
            }
        }

        private static IList<SampleEntity> CreateSampleEntities(int count) {
            var result = new List<SampleEntity>(count);
            for (var i = 0; i < count; i++) {
                result.Add(new SampleEntity {
                    SubId = i + 1,
                    Title = $"title for {i+1}",
                    Text = "NHibernate."
                });
            }
            return result;
        }

    }

}
