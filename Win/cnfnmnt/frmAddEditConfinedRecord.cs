﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Helpers;
using Models;
using Models.Repository;

namespace Win.cnfnmnt
{
    public partial class frmAddEditConfinedRecord : DevExpress.XtraEditors.XtraForm
    {
        private MethodType methodType;
        private ConfinementRecords oldValue;
        private DateTime? DateCreated;
        public frmAddEditConfinedRecord(ConfinementRecords oldValue, MethodType methodType)
        {
            InitializeComponent();
            // This line of code is generated by Data Source Configuration Wizard
            OwnerBinding.QueryableSource = new Models.ModelDb().Owners;
            VetBinding.QueryableSource = new UnitOfWork().VeterinariesRepo.Fetch();
            Details(oldValue);
            this.methodType = methodType;
            this.oldValue = oldValue;
        }

        private void gridLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            var owner = cboOwner.GetSelectedDataRow() as Owners;
            entityServerModeSource2.QueryableSource = new Models.ModelDb().Patients.Where(x => x.OwnerId == owner.Id);
            txtAddress.Text = owner?.Address;
            txtTelNo.Text = owner?.TelNo;
        }

        private void cboPetName_EditValueChanged(object sender, EventArgs e)
        {
            var patients = cboPetName.GetSelectedDataRow() as Patients;
            txtSpecies.EditValue = patients?.Species;
            txtColor.EditValue = patients?.Color;
        }

        private void frmAddEditConfinedRecord_Load(object sender, EventArgs e)
        {

        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("Do you want to submit this?", "Submit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            if (this.methodType == MethodType.Add)
                Add(confinementRecords());
            else
                Edit(confinementRecords());
            this.Close();
        }

        void Add(ConfinementRecords item)
        {

            try
            {
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    item.DateCreated = DateTime.Now;
                    item.CreatedBy = User.UserId;
                    unitOfWork.ConfinementRecordsRepo.Insert(item);
                    unitOfWork.Save();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        void Edit(ConfinementRecords item)
        {
            try
            {
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    item.Id = oldValue.Id;
                    item.DateCreated = oldValue.DateCreated;
                    item.CreatedBy = oldValue.CreatedBy;
                    unitOfWork.ConfinementRecordsRepo.Update(item);
                    unitOfWork.Save();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void Details(ConfinementRecords item)
        {
            if (item == null)
                return;
            try
            {
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {

                    this.oldValue = item;
                    this.cboOwner.EditValue = item.OwnerId;
                    this.txtObservation.EditValue = item.Observations;
                    this.cboAttendingVet.EditValue = item.VetId;

                    entityServerModeSource2.QueryableSource = new Models.ModelDb().Patients.Where(x => x.OwnerId == item.OwnerId);
                    this.cboPetName.EditValue = item.PatientId;
                    this.txtColor.Text = item.Patients?.Color;
                    this.txtSpecies.Text = item.Patients?.Species;
                    // var owner = cboOwner.GetSelectedDataRow() as Owners;
                    txtAddress.Text = item.Patients?.Owners?.Address;
                    txtTelNo.Text = item.Patients?.Owners?.TelNo;
                    txtMedication.Text = item.Medication;
                    comboBoxEdit1.EditValue = item.Status;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        ConfinementRecords confinementRecords()
        {
            return new ConfinementRecords()
            {
                Id = this.Tag.ToInt(),
                OwnerId = cboOwner.EditValue.ToInt(),
                PatientId = cboPetName.EditValue.ToInt(),
                Observations = txtObservation.Text,
                VetId = cboAttendingVet.EditValue.ToInt(),
                Medication = txtMedication.Text,
                Status = comboBoxEdit1.Text.ToString()
            };
        }
    }
}