﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Xml;

namespace UsrControlTemplate
{
    /// <summary>
    /// The PaddedGrid control is a Grid that supports padding.
    /// </summary>
    public class PaddedGrid : Grid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaddedGrid"/> class.
        /// </summary>
        public PaddedGrid()
        {
            //  Add a loded event handler.
            Loaded += new RoutedEventHandler(PaddedGrid_Loaded);
        }

        /// <summary>
        /// Handles the Loaded event of the PaddedGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void PaddedGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //  Get the number of children.
            int childCount = VisualTreeHelper.GetChildrenCount(this);

            //  Go through the children.
            for (int i = 0; i < childCount; i++)
            {
                //  Get the child.
                DependencyObject child = VisualTreeHelper.GetChild(this, i);

                //  Try and get the margin property.
                DependencyProperty marginProperty = GetMarginProperty(child);

                //  If we have a margin property, bind it to the padding.
                if (marginProperty != null)
                {
                    //  Create the binding.
                    Binding binding = new Binding();
                    binding.Source = this;
                    binding.Path = new PropertyPath("Padding");

                    //  Bind the child's margin to the grid's padding.
                    BindingOperations.SetBinding(child, marginProperty, binding);
                }
            }
        }

        /// 在這個方法實作Grid的格線
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        protected override void OnRender(DrawingContext dc)
        {
            bool isFirst = true;
            double topOffset = 0;
            Pen pen = new Pen(Brushes.Black, 0.5);
            pen.Freeze();

            
            foreach (RowDefinition row in this.RowDefinitions)
            {
                if (!isFirst)
                    dc.DrawLine(pen, new Point(0, topOffset), new Point(this.ActualWidth, topOffset));
                else
                    isFirst = false;

                //topOffset += 75;
                topOffset += row.ActualHeight;
            }

            base.OnRender(dc);
        }

        /// <summary>
        /// Gets the margin property of a dependency object (if it has one).
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>The margin property of the dependency object, or null if one doesn't exist.</returns>
        protected static DependencyProperty GetMarginProperty(DependencyObject dependencyObject)
        {
            //  Go through each property for the object.
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(dependencyObject))
            {
                //  Get the dependency property descriptor.
                DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(propertyDescriptor);

                //  Have we found the margin?
                if (dpd != null && dpd.Name == "Margin")
                {
                    //  We've found the margin property, return it.
                    return dpd.DependencyProperty;
                }
            }

            //  Failed to find the margin, return null.
            return null;
        }

        /// <summary>
        /// Called when the padding changes.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnPaddingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            //  Get the padded grid that has had its padding changed.
            PaddedGrid paddedGrid = dependencyObject as PaddedGrid;

            //  Force the layout to be updated.
            paddedGrid.UpdateLayout();
        }

        /// <summary>
        /// The internal dependency property object for the 'Padding' property.
        /// </summary>
        private static readonly DependencyProperty PaddingProperty =
          DependencyProperty.Register("Padding", typeof(Thickness), typeof(PaddedGrid),
          new UIPropertyMetadata(new Thickness(0.0), new PropertyChangedCallback(OnPaddingChanged)));

        /// <summary>
        /// Gets or sets the padding.
        /// </summary>
        /// <value>The padding.</value>
        [Description("The padding property."), Category("Common Properties")]
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }
    }
}