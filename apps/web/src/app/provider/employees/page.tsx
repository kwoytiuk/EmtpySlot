'use client';

import { useState, useEffect } from 'react';
import { employeesApi, type Employee, type EmployeeSchedule } from 'shared';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';

export default function EmployeesPage() {
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [loading, setLoading] = useState(true);
  const [showAddModal, setShowAddModal] = useState(false);
  const [selectedEmployee, setSelectedEmployee] = useState<Employee | null>(null);
  const [showScheduleModal, setShowScheduleModal] = useState(false);

  useEffect(() => {
    loadEmployees();
  }, []);

  const loadEmployees = async () => {
    try {
      const emps = await employeesApi.getProviderEmployees();
      setEmployees(emps);
    } catch (error) {
      console.error('Error loading employees:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleAddEmployee = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const formData = new FormData(e.currentTarget);

    try {
      const { error } = await employeesApi.createEmployee({
        first_name: formData.get('first_name') as string,
        last_name: formData.get('last_name') as string,
        email: formData.get('email') as string,
        phone: formData.get('phone') as string,
        position: formData.get('position') as string,
      });

      if (!error) {
        setShowAddModal(false);
        loadEmployees();
      }
    } catch (error) {
      console.error('Error adding employee:', error);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Team Management</h1>
          <p className="text-gray-600 mt-2">
            Manage your staff and their schedules
          </p>
        </div>

        {/* Add Employee Button */}
        <div className="mb-6">
          <Button
            onClick={() => setShowAddModal(true)}
            className="bg-blue-600 hover:bg-blue-700"
          >
            + Add Employee
          </Button>
        </div>

        {/* Employees Grid */}
        {employees.length === 0 ? (
          <div className="bg-white rounded-xl border border-gray-200 p-12 text-center">
            <div className="text-6xl mb-4">👥</div>
            <h3 className="text-xl font-semibold text-gray-900 mb-2">
              No employees yet
            </h3>
            <p className="text-gray-600 mb-6">
              Add your first team member to get started
            </p>
            <Button
              onClick={() => setShowAddModal(true)}
              className="bg-blue-600 hover:bg-blue-700"
            >
              Add Employee
            </Button>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {employees.map((employee) => (
              <div
                key={employee.id}
                className="bg-white rounded-xl border border-gray-200 p-6 hover:shadow-lg transition-shadow"
              >
                {/* Employee Avatar */}
                <div className="flex items-center mb-4">
                  <div className="w-16 h-16 bg-gradient-to-br from-blue-500 to-purple-600 rounded-full flex items-center justify-center text-white text-2xl font-bold">
                    {employee.first_name[0]}{employee.last_name[0]}
                  </div>
                  <div className="ml-4">
                    <h3 className="font-bold text-lg text-gray-900">
                      {employee.first_name} {employee.last_name}
                    </h3>
                    {employee.position && (
                      <p className="text-sm text-gray-600">{employee.position}</p>
                    )}
                  </div>
                </div>

                {/* Contact Info */}
                <div className="space-y-2 mb-4">
                  <p className="text-sm text-gray-600 flex items-center">
                    <span className="mr-2">📧</span>
                    {employee.email}
                  </p>
                  {employee.phone && (
                    <p className="text-sm text-gray-600 flex items-center">
                      <span className="mr-2">📞</span>
                      {employee.phone}
                    </p>
                  )}
                </div>

                {/* Actions */}
                <div className="flex gap-2">
                  <Button
                    variant="outline"
                    size="sm"
                    className="flex-1"
                    onClick={() => {
                      setSelectedEmployee(employee);
                      setShowScheduleModal(true);
                    }}
                  >
                    Schedule
                  </Button>
                  <Button
                    variant="outline"
                    size="sm"
                    className="flex-1 text-red-600 hover:text-red-700"
                    onClick={async () => {
                      if (confirm('Deactivate this employee?')) {
                        await employeesApi.deactivateEmployee(employee.id);
                        loadEmployees();
                      }
                    }}
                  >
                    Deactivate
                  </Button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Add Employee Modal */}
      {showAddModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-xl max-w-md w-full p-6">
            <h2 className="text-2xl font-bold mb-6">Add Employee</h2>
            <form onSubmit={handleAddEmployee} className="space-y-4">
              <div>
                <Label htmlFor="first_name">First Name</Label>
                <Input id="first_name" name="first_name" required />
              </div>
              <div>
                <Label htmlFor="last_name">Last Name</Label>
                <Input id="last_name" name="last_name" required />
              </div>
              <div>
                <Label htmlFor="email">Email</Label>
                <Input id="email" name="email" type="email" required />
              </div>
              <div>
                <Label htmlFor="phone">Phone</Label>
                <Input id="phone" name="phone" type="tel" />
              </div>
              <div>
                <Label htmlFor="position">Position/Title</Label>
                <Input id="position" name="position" placeholder="e.g., Stylist, Technician" />
              </div>
              <div className="flex gap-3 pt-4">
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => setShowAddModal(false)}
                  className="flex-1"
                >
                  Cancel
                </Button>
                <Button type="submit" className="flex-1 bg-blue-600 hover:bg-blue-700">
                  Add Employee
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Schedule Modal */}
      {showScheduleModal && selectedEmployee && (
        <ScheduleModal
          employee={selectedEmployee}
          onClose={() => {
            setShowScheduleModal(false);
            setSelectedEmployee(null);
          }}
        />
      )}
    </div>
  );
}

function ScheduleModal({
  employee,
  onClose,
}: {
  employee: Employee;
  onClose: () => void;
}) {
  const [schedules, setSchedules] = useState<EmployeeSchedule[]>([]);
  const [loading, setLoading] = useState(true);

  const days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

  useEffect(() => {
    loadSchedules();
  }, []);

  const loadSchedules = async () => {
    try {
      const scheds = await employeesApi.getEmployeeSchedule(employee.id);
      setSchedules(scheds);
    } catch (error) {
      console.error('Error loading schedules:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleAddSchedule = async (dayOfWeek: number) => {
    const startTime = prompt('Start time (HH:MM format, e.g., 09:00):');
    const endTime = prompt('End time (HH:MM format, e.g., 17:00):');

    if (startTime && endTime) {
      await employeesApi.upsertEmployeeSchedule({
        employee_id: employee.id,
        day_of_week: dayOfWeek,
        start_time: startTime,
        end_time: endTime,
      });
      loadSchedules();
    }
  };

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center p-4 z-50">
      <div className="bg-white rounded-xl max-w-2xl w-full p-6 max-h-[90vh] overflow-y-auto">
        <h2 className="text-2xl font-bold mb-2">
          {employee.first_name}'s Schedule
        </h2>
        <p className="text-gray-600 mb-6">Set weekly working hours</p>

        {loading ? (
          <div className="text-center py-8">Loading...</div>
        ) : (
          <div className="space-y-4">
            {days.map((day, index) => {
              const daySchedules = schedules.filter((s) => s.day_of_week === index);
              return (
                <div key={index} className="border border-gray-200 rounded-lg p-4">
                  <div className="flex justify-between items-center mb-2">
                    <h3 className="font-semibold text-gray-900">{day}</h3>
                    <Button
                      size="sm"
                      variant="outline"
                      onClick={() => handleAddSchedule(index)}
                    >
                      + Add Hours
                    </Button>
                  </div>
                  {daySchedules.length === 0 ? (
                    <p className="text-sm text-gray-500">Not available</p>
                  ) : (
                    <div className="space-y-2">
                      {daySchedules.map((schedule) => (
                        <div
                          key={schedule.id}
                          className="flex justify-between items-center text-sm"
                        >
                          <span className="text-gray-700">
                            {schedule.start_time} - {schedule.end_time}
                          </span>
                          <button
                            onClick={async () => {
                              await employeesApi.deleteEmployeeSchedule(schedule.id);
                              loadSchedules();
                            }}
                            className="text-red-600 hover:text-red-700"
                          >
                            Remove
                          </button>
                        </div>
                      ))}
                    </div>
                  )}
                </div>
              );
            })}
          </div>
        )}

        <div className="mt-6">
          <Button onClick={onClose} variant="outline" className="w-full">
            Close
          </Button>
        </div>
      </div>
    </div>
  );
}
