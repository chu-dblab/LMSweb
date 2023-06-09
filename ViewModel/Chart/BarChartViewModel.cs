using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMSweb.ViewModel.Chart
{
//    {
//            type: 'bar',
//            data: {
//                labels: ['Red', 'Blue', 'Yellow', 'Green', 'Purple', 'Orange'],
//                datasets: [{
//                    label: '# of Votes',
//                    data: [12, 19, 3, 5, 2, 3],
//                    backgroundColor: [
//                        'rgba(255, 99, 132, 0.2)',
//                        'rgba(54, 162, 235, 0.2)',
//                        'rgba(255, 206, 86, 0.2)',
//                        'rgba(75, 192, 192, 0.2)',
//                        'rgba(153, 102, 255, 0.2)',
//                        'rgba(255, 159, 64, 0.2)'
//                    ],
//                    borderColor: [
//                        'rgba(255, 99, 132, 1)',
//                        'rgba(54, 162, 235, 1)',
//                        'rgba(255, 206, 86, 1)',
//                        'rgba(75, 192, 192, 1)',
//                        'rgba(153, 102, 255, 1)',
//                        'rgba(255, 159, 64, 1)'
//                    ],
//                    borderWidth: 1
//                }]
//            },
//            options:
//{
//scales:
//    {
//    y:
//        {
//        beginAtZero: true
//                    }
//    }
//}
//        }

    public class BarChartViewModel
    {
        public string labels { get; set; }
        public List<DataSets> datasets { get; set; }

    }

    public class DataSets
    {
        public string label { get; set; }
        public string data { get; set; }
        public string backgroundColor { get; set; }
        public string borderColor { get; set; }
        public string borderWidth { get; set; }
    }
}