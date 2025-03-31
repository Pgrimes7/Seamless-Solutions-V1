document.addEventListener("DOMContentLoaded", function () {

    //donut chart
    var grantsCtx = document.getElementById('grantsChart').getContext('2d');
    var grantsChart = new Chart(grantsCtx, {
        type: 'doughnut',
        data: {
            labels: ['Active', 'Potential', 'Funded', 'Archived'],
            datasets: [{
                label: 'Grant Status',
                data: [10, 5, 15, 30],
                backgroundColor: [
                    '#450084', //active
                    '#CBB677', //potential
                    '#F4EFE1', //funded
                    '#DACCE6'  //archived
                ],
                hoverOffset: 4
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            cutout: '70%'
        }
    });

});
