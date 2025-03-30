document.addEventListener("DOMContentLoaded", function () {

    //donut chart
    var grantsCtx = document.getElementById('grantsChart').getContext('2d');
    var grantsChart = new Chart(grantsCtx, {
        type: 'doughnut',
        data: {
            labels: ['Pending', 'Not Started', 'Completed'],
            datasets: [{
                label: 'Grant Status',
                data: [10, 5, 15],
                backgroundColor: [
                    '#ADD8E6', //pending
                    '#FFD700', //not started
                    'Green' //completed
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

    //remove and add assignee in create grant modal
    let assigneeCounter = 1; // Counter to track number of assignees

    const addAssigneeBtn = document.getElementById("addAssigneeBtn");
    const assigneeContainer = document.getElementById("assigneeContainer");

    // Function to add a new assignee section
    addAssigneeBtn.addEventListener("click", function () {
        assigneeCounter++; // Increment counter for each new assignee

        // Clone the first assignee section and update IDs and names
        const newAssignee = document.getElementById("assignee-1").cloneNode(true);
        newAssignee.id = `assignee-${assigneeCounter}`; // Give the cloned section a unique ID

        // Clear input values for the cloned section
        const inputs = newAssignee.querySelectorAll('input');
        inputs.forEach((input) => {
            input.value = ''; // Clear input values
        });

        // Update input IDs and names for proper form handling
        inputs.forEach((input) => {
            const baseName = input.name.split('[]')[0];
            input.id = `${baseName}-${assigneeCounter}`; // Add assignee counter to make the ID unique
            input.name = `${baseName}[]`; // Ensure the name is an array for multiple values
        });

        // Add event listener for the remove button
        const removeButton = newAssignee.querySelector(".remove-assignee-btn");
        removeButton.addEventListener("click", function () {
            removeAssignee(removeButton);
        });

        // Append the new assignee section to the container
        assigneeContainer.appendChild(newAssignee);
    });

    // Function to remove assignee section
    function removeAssignee(button) {
        const assigneeSection = button.closest(".assignee-section");

        if (assigneeCounter > 1) { // Only allow removal if there's more than 1 assignee
            assigneeSection.remove();
            assigneeCounter--; // Decrement the counter when an assignee is removed
        } else {
            alert("At least one assignee is required.");
        }
    }

    // Add event listener for the first assignee's remove button (if already present)
    const initialRemoveButton = document.querySelector(".remove-assignee-btn");
    if (initialRemoveButton) {
        initialRemoveButton.addEventListener("click", function () {
            removeAssignee(initialRemoveButton);
        });
    }

});
