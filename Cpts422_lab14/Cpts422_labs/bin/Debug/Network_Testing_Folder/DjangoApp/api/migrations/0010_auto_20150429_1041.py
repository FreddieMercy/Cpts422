# -*- coding: utf-8 -*-
from __future__ import unicode_literals

from django.db import models, migrations


class Migration(migrations.Migration):

    dependencies = [
        ('api', '0009_auto_20150429_1004'),
    ]

    operations = [
        migrations.AlterField(
            model_name='api',
            name='pub_date',
            field=models.DateTimeField(),
            preserve_default=True,
        ),
    ]
