document.addEventListener("DOMContentLoaded", function () {

    //donut chart
    fetch('/Dashboard?handler=GrantStatusData')
        .then(response => response.json())
        .then(data => {
            var grantsCtx = document.getElementById('grantsChart').getContext('2d');
            var grantsChart = new Chart(grantsCtx, {
                type: 'doughnut',
                data: {
                    labels: ['Active', 'Potential', 'Funded', 'Archived', 'Rejected'],
                    datasets: [{
                        label: 'Grant Status',
                        data: data,
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
                    cutout: '70%',
                    plugins: {
                        title: {
                            display: true,
                            text: 'Grant Status',
                            font: {
                                size: 17
                            },

                        }
                    }
                }
            });

        });
})
