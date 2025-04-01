document.addEventListener("DOMContentLoaded", function () {

    //donut chart
    var grantsCtx = document.getElementById('grantsChart').getContext('2d');
    var grantsChart = new Chart(grantsCtx, {
        type: 'doughnut',
        data: {
            labels: ['Active', 'Potential', 'Funded', 'Archived', 'Rejected'],
            datasets: [{
                label: 'Grant Status',
                data: [10, 5, 15, 30, 3],
                backgroundColor: [
                    '#450084', //active, purple
                    '#F4EFE1', //potential, light gold
                    '#CBB677', //funded, gold
                    '#DACCE6',  //archived, light purple
                    '#BF0603'  //rejected, red
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
