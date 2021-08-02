using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAsyncWebApp
{
    public class TaskManager
    {
        public List<Task> _taskList;
        public int _total;


        public void Setup(int count)
        {
            _total = count;
            _taskList = new List<Task>(count);
        }


        public void Start(Action doing)
        {
            for (int i = 0; i < _total; i++)
            {
                var task = Task.Factory.StartNew(doing);
                _taskList.Add(task);
            }
        }


        public void Start(Action<int> doing)
        {
            for (int i = 0; i < _total; i++)
            {
                var task = Task.Factory.StartNew((index) => doing((int)index), i);
                _taskList.Add(task);
            }
        }


        public void Wait()
        {
            Task.WaitAll(_taskList.ToArray());
        }
    }
}
