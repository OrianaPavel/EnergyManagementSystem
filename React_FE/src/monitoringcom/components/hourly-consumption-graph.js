import React, { Component } from 'react';
import { Line } from 'react-chartjs-2';
import { getHourlyConsumption } from '../api/monitoringcom-api';
import { Chart, registerables } from 'chart.js';
Chart.register(...registerables);

class HourlyConsumptionGraph extends Component {
    constructor(props) {
        super(props);
        this.state = {
            hourlyData: {},
            loading: true,
            error: null,
            selectedDate: new Date().toISOString().slice(0, 10) // Default to current date
        };
    }

    componentDidMount() {
        const { match } = this.props;
        const deviceId = match.params.deviceId;
        this.fetchHourlyData(deviceId, this.state.selectedDate);
    }

    fetchHourlyData(deviceId, date) {
        this.setState({ loading: true });
        getHourlyConsumption(deviceId, date,(result, status, error) => {
            if (status === 200) {
                //console.log("Datele pt grafic sunt: " + JSON.stringify(result, null, 2));
                this.setState({ hourlyData: result, loading: false });
            } else {
                this.setState({ error: 'Error fetching data or no data available', loading: false });
            }
        });
    }

    handleDateChange = (event) => {
        const newDate = event.target.value;
        this.setState({ selectedDate: newDate });
        const { match } = this.props;
        const deviceId = match.params.deviceId;
        this.fetchHourlyData(deviceId, newDate);
    }

    renderChart() {
        const { hourlyData } = this.state;
        const data = {
            labels: Object.keys(hourlyData).map(hour => `${hour}:00`),
            datasets: [
                {
                    label: 'Hourly Consumption',
                    data: Object.values(hourlyData),
                    fill: false,
                    backgroundColor: 'rgb(75, 192, 192)',
                    borderColor: 'rgba(75, 192, 192, 0.2)',
                },
            ],
        };

        const options = {
            scales: {
                x: { 
                    type: 'category', 
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Hour'
                    }
                },
                y: { 
                    type: 'linear', 
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Consumption'
                    }
                }
            },
            plugins: {
                legend: {
                    display: true
                },
                title: {
                    display: true,
                    text: 'Hourly Consumption'
                }
            }
        };
        

        return <Line data={data} options={options} />;
    }

    render() {
        const { loading, error, selectedDate } = this.state;

        if (loading) {
            return <p>Loading...</p>;
        }

        if (error) {
            return <p>{error}</p>;
        }

        return (
            <div>
                <input type="date" value={selectedDate} onChange={this.handleDateChange} />
                {this.renderChart()}
            </div>
        );
    }
}

export default HourlyConsumptionGraph;
